using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Noggog;
using System.Threading;

namespace Noggog
{
    public static class ObservableExt
    {
        public static IDisposable Subscribe<T>(this IObservable<T> source, Action action)
        {
            return source.Subscribe((i) => action());
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Func<Task> action)
        {
            return source
                .SelectMany(async i =>
                {
                    await action().ConfigureAwait(false);
                    return System.Reactive.Unit.Default;
                })
                .Subscribe();
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Func<T, Task> action)
        {
            return source
                .SelectMany(async i =>
                {
                    await action(i).ConfigureAwait(false);
                    return System.Reactive.Unit.Default;
                })
                .Subscribe();
        }

        public static IObservable<(T Previous, T Current)> Pairwise<T>(this IObservable<T> source)
        {
            T prevStorage = default;
            return source.Select(i =>
            {
                var prev = prevStorage;
                prevStorage = i;
                return (prev, i);
            });
        }

        public static IObservable<R> Cast<T, R>(this IObservable<T> source)
            where T : R
        {
            return source.Select<T, R>(x => x);
        }

        public static IObservable<Unit> SelectTask<T>(this IObservable<T> source, Func<T, Task> task)
        {
            return source
                .SelectMany(async i =>
                {
                    await task(i).ConfigureAwait(false);
                    return System.Reactive.Unit.Default;
                });
        }

        public static IObservable<Unit> SelectTask<T>(this IObservable<T> source, Func<Task> task)
        {
            return source
                .SelectMany(async _ =>
                {
                    await task().ConfigureAwait(false);
                    return System.Reactive.Unit.Default;
                });
        }

        public static IObservable<R> SelectTask<T, R>(this IObservable<T> source, Func<Task<R>> task)
        {
            return source
                .SelectMany(_ => task());
        }

        public static IObservable<R> SelectTask<T, R>(this IObservable<T> source, Func<T, Task<R>> task)
        {
            return source
                .SelectMany(async i =>
                {
                    return await task(i).ConfigureAwait(false);
                });
        }

        public static IObservable<T> FilterSwitch<T>(this IObservable<T> source, IObservable<bool> filterSwitch)
        {
            return filterSwitch
                .Select(on =>
                {
                    if (on)
                    {
                        return source;
                    }
                    else
                    {
                        return Observable.Empty<T>();
                    }
                })
                .Switch();
        }

        public static IObservable<T> FilterSwitch<T>(this IObservable<T> source, IObservable<bool> filterSwitch, T fallback)
        {
            return filterSwitch
                .Select(on =>
                {
                    if (on)
                    {
                        return source;
                    }
                    else
                    {
                        return Observable.Return<T>(fallback);
                    }
                })
                .Switch();
        }

        public static IObservable<Unit> Unit<T>(this IObservable<T> source)
        {
            return source.Select(u => System.Reactive.Unit.Default);
        }

        public static IObservable<T> NotNull<T>(this IObservable<T?> source)
            where T : class
        {
            return source.Where(u => u != null)!;
        }

        public static IObservable<TSource> PublishRefCount<TSource>(this IObservable<TSource> source)
        {
            return source.Publish().RefCount();
        }

        public static IObservable<TSource> DisposeWith<TSource>(this IObservable<TSource> source, CompositeDisposable composite)
            where TSource : IDisposable
        {
            SerialDisposable serialDisposable = new SerialDisposable();
            composite.Add(serialDisposable);
            return source.Do(
                (item) =>
                {
                    serialDisposable.Disposable = item;
                });
        }

        public static IObservable<TRet> SelectLatestFrom<TSource, TRet>(this IObservable<TSource> source, IObservable<TRet> from)
        {
            return source.WithLatestFrom(
                from,
                resultSelector: (s, f) => f);
        }

        public static IObservable<TRet> SelectLatest<TSource, TRet>(this IObservable<TSource> source, IObservable<TRet> from)
        {
            return source.CombineLatest(
                from,
                resultSelector: (s, f) => f);
        }

        /// ToDo:
        /// Can probably improve to fire final percent early, if last period is smaller than pulseSpan
        public static IObservable<Percent> ProgressInterval(DateTime startTime, DateTime endTime, TimeSpan pulseSpan)
        {
            if (startTime >= endTime) return Observable.Return(Percent.One);
            var startingDiffMS = (endTime - startTime).TotalMilliseconds * 1.0;
            Percent CalculatePercent(DateTime now, DateTime end)
            {
                var diff = end - now;
                if (diff.TotalMilliseconds < 0) return Percent.One;
                return new Percent((startingDiffMS - diff.TotalMilliseconds) / startingDiffMS);
            }
            return Observable.Interval(pulseSpan)
                .Select(_ => CalculatePercent(DateTime.Now, endTime))
                .StartWith(CalculatePercent(DateTime.Now, endTime));
        }

        /// Inspiration:
        /// http://reactivex.io/documentation/operators/debounce.html
        /// https://stackoverflow.com/questions/20034476/how-can-i-use-reactive-extensions-to-throttle-events-using-a-max-window-size
        public static IObservable<T> Debounce<T>(this IObservable<T> source, TimeSpan interval, IScheduler scheduler)
        {
            return Observable.Create<T>(o =>
            {
                var hasValue = false;
                bool throttling = false;
                T value = default;

                var dueTimeDisposable = new SerialDisposable();

                void internalCallback()
                {
                    if (hasValue)
                    {
                        // We have another value that came in to fire.
                        // Reregister for callback
                        dueTimeDisposable.Disposable = scheduler.Schedule(interval, internalCallback);
                        o.OnNext(value);
                        value = default;
                        hasValue = false;
                    }
                    else
                    {
                        // Nothing to do, throttle is complete.
                        throttling = false;
                    }
                }

                return source.Subscribe(
                    onNext: (x) =>
                    {
                        if (!throttling)
                        {
                            // Fire initial value
                            o.OnNext(x);
                            // Mark that we're throttling
                            throttling = true;
                            // Register for callback when throttle is complete
                            dueTimeDisposable.Disposable = scheduler.Schedule(interval, internalCallback);
                        }
                        else
                        {
                            // In the middle of throttle
                            // Save value and return
                            hasValue = true;
                            value = x;
                        }
                    },
                    onError: o.OnError,
                    onCompleted: o.OnCompleted);
            });
        }

        public static IObservable<T> DoTask<T>(this IObservable<T> source, Func<T, Task> task)
        {
            return source
                .SelectMany(async (x) =>
                {
                    await task(x).ConfigureAwait(false);
                    return x;
                });
        }

        public static IObservable<Unit> TurnedOff(this IObservable<bool> source)
        {
            return source
                .DistinctUntilChanged()
                .Pairwise()
                .Where(x => x.Previous && !x.Current)
                .Unit();
        }

        public static IObservable<Unit> TurnedOn(this IObservable<bool> source)
        {
            return source
                .DistinctUntilChanged()
                .Pairwise()
                .Where(x => !x.Previous && x.Current)
                .Unit();
        }

        public static IObservable<TimeSpan> TimePassed(TimeSpan notificationFrequency, IScheduler scheduler)
        {
            return Observable.Return(System.Reactive.Unit.Default)
                .Select(_ =>
                {
                    return Observable.CombineLatest(
                        Observable.Interval(notificationFrequency, scheduler),
                        Observable.Return(DateTime.Now),
                        (_, startTime) =>
                        {
                            return DateTime.Now - startTime;
                        });
                })
                .Switch();
        }

        /// <summary>
        /// A convenience function to make a flip flop observable that triggers whenever the source observable changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obs">Source observable</param>
        /// <param name="span">How long to delay before flopping back to false</param>
        /// <returns>Observable that turns true when source observable changes, then back to false after a time period</returns>
        public static IObservable<bool> FlipFlop<T>(this IObservable<T> obs, TimeSpan span)
        {
            return Observable.Merge(
                obs.Select(f => true),
                obs.Delay(span).Select(f => false));
        }

        // ToDo
        // Have T implement IDisposable once resulting nullability errors can be dealt with
        public static IObservable<T> DisposePrevious<T>(this IObservable<T> obs)
        {
            return obs
                .StartWith(default(T)!)
                .Pairwise()
                .Do(x =>
                {
                    if (x.Previous is IDisposable disp)
                    {
                        disp.Dispose();
                    }
                })
                .Select(x => x.Current);
        }

        public static IObservable<TRet> SelectReplace<TInput, TRet>(
            this IObservable<TInput> obs,
            Func<TInput, CancellationToken, Task<TRet>> func,
            bool catchTaskCancelled = true)
        {
            return obs.Select(i => Observable.DeferAsync(async token =>
            {
                try
                {
                    return Observable.Return((true, await func(i, token).ConfigureAwait(false)));
                }
                catch (TaskCanceledException)
                when (catchTaskCancelled)
                {
                    return Observable.Return((false, default(TRet)));
                }
            }))
            .Switch()
            .Where(x => x.Item1)
            .Select(x => x.Item2);
        }

        public static IObservable<TRet> SelectReplaceWithIntermediate<TInput, TRet>(
            this IObservable<TInput> obs,
            TRet newItemIntermediate,
            Func<TInput, CancellationToken, Task<TRet>> func,
            bool catchTaskCancelled = true)
        {
            return obs.Select(i =>
            {
                return Observable.Return((true, newItemIntermediate))
                    .Concat(Observable.DeferAsync(async token =>
                    {
                        try
                        {
                            return Observable.Return((true, await func(i, token).ConfigureAwait(false)));
                        }
                        catch (TaskCanceledException)
                        when (catchTaskCancelled)
                        {
                            return Observable.Return((false, default(TRet)));
                        }
                    }));
            })
            .Switch()
            .Where(x => x.Item1)
            .Select(x => x.Item2);
        }

        public static IObservable<T> ReplayMostRecent<T>(this IObservable<T> obs, IObservable<Unit> signal)
        {
            return obs.Merge(
                signal.WithLatestFrom(
                    obs,
                    (_, o) => o));
        }

        private class DisposableWrapper<TResource> : IDisposable
            where TResource : IDisposable
        {
            public GetResponse<TResource> Item;

            public void Dispose()
            {
                Item.Value?.Dispose();
            }
        }

        public static IObservable<TResult> UsingWithCatch<TResult, TResource>(
            Func<TResource> resourceFactory, 
            Func<GetResponse<TResource>, IObservable<TResult>> observableFactory) 
            where TResource : IDisposable
        {
            return Observable.Using(
                () =>
                {
                    try
                    {
                        return new DisposableWrapper<TResource>()
                        {
                            Item = GetResponse<TResource>.Succeed(resourceFactory())
                        };
                    }
                    catch (Exception ex)
                    {
                        return new DisposableWrapper<TResource>()
                        {
                            Item = GetResponse<TResource>.Fail(ex)
                        };
                    }
                },
                (resource) =>
                {
                    return observableFactory(resource.Item);
                });
        }
    }
}
