using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using CSharpExt.Rx;
using System.Reactive.Linq;
using ReactiveUI;
using System.Windows.Input;

namespace System
{
    public static class ObservableExt
    {
        // https://stackoverflow.com/questions/24790191/hot-concat-in-rx
        public static IObservable<T> HotConcat<T>(params IObservable<T>[] sources)
        {
            var s2 = sources.Select(s => s.BufferUntilSubscribed());
            var subscriptions = new CompositeDisposable(s2.Select(s3 => s3.Connect()).ToArray());
            return Observable.Create<T>(observer =>
            {
                var s = new SingleAssignmentDisposable();
                var d = new CompositeDisposable(subscriptions.ToArray());
                d.Add(s);

                s.Disposable = s2.Concat().Subscribe(observer);

                return d;
            });
        }

        /// <summary>
        /// Returns a connectable observable, that once connected, will start buffering data until the observer subscribes, at which time it will send all buffered data to the observer and then start sending new data.
        /// Thus the observer may subscribe late to a hot observable yet still see all of the data.  Later observers will not see the buffered events.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="scheduler">Scheduler to use to dump the buffered data to the observer.</param>
        /// <returns></returns>
        public static IConnectableObservable<T> BufferUntilSubscribed<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new BufferUntilSubscribedObservable<T>(source, scheduler);
        }

        /// <summary>
        /// Returns a connectable observable, that once connected, will start buffering data until the observer subscribes, at which time it will send all buffered data to the observer and then start sending new data.
        /// Thus the observer may subscribe late to a hot observable yet still see all of the data.  Later observers will not see the buffered events.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IConnectableObservable<T> BufferUntilSubscribed<T>(this IObservable<T> source)
        {
            return new BufferUntilSubscribedObservable<T>(source, Scheduler.Immediate);
        }

        public static IObservable<T> MakeHot<T>(this IObservable<T> source)
        {
            var subject = new ReplaySubject<T>();
            source.Subscribe(subject);
            return Observable.Create<T>((observer) => subject.Subscribe(observer));
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Action action)
        {
            return source.Subscribe((i) => action());
        }

        public static IDisposable Subscribe<T>(this IObservable<T> source, Func<Task> action)
        {
            return source
                .SelectMany(async i =>
                {
                    await action();
                    return System.Reactive.Unit.Default;
                })
                .Subscribe();
        }

        public static IObservable<(T Previous, T Current)> WithPrevious<T>(this IObservable<T> source)
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

        public static IObservable<Unit> Select<T>(this IObservable<T> source, Func<Task> task)
        {
            return source
                .SelectMany(async _ =>
                {
                    await task().ConfigureAwait(false);
                    return System.Reactive.Unit.Default;
                });
        }

        public static IObservable<R> Select<T, R>(this IObservable<T> source, Func<Task<R>> task)
        {
            return source
                .SelectMany(_ => task());
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

        public static IObservable<Unit> Unit<T>(this IObservable<T> source)
        {
            return source.Select(u => System.Reactive.Unit.Default);
        }

        public static IObservable<T> NotNull<T>(this IObservable<T> source)
            where T : class
        {
            return source.Where(u => u != null);
        }

        public static IDisposable InvokeCommand<T>(this IObservable<T> item, IReactiveCommand command)
        {
            return ReactiveUI.ReactiveCommandMixins.InvokeCommand(item, (ICommand)command);
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
    }
}
