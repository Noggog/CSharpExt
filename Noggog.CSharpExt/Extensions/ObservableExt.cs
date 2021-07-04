using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Reactive.Disposables;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.IO.Abstractions;
using DynamicData;
using DynamicData.Kernel;
using Path = System.IO.Path;
using RenamedEventHandler = System.IO.RenamedEventHandler;
using FileSystemEventArgs = System.IO.FileSystemEventArgs;
using FileSystemEventHandler = System.IO.FileSystemEventHandler;
using WatcherChangeTypes = System.IO.WatcherChangeTypes;
using RenamedEventArgs = System.IO.RenamedEventArgs;

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

        public static IObservable<(T? Previous, T Current)> Pairwise<T>(this IObservable<T> source)
        {
            T? prevStorage = default;
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

        public static IObservable<R> WhereCastable<T, R>(this IObservable<T> source)
            where T : class
            where R : class
        {
            return source.Select(x => x as R)
                .NotNull();
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

        public static IObservable<T> NotNull<T>(this IObservable<T?> source)
            where T : struct
        {
            return source.Where(u => u != null).Select(x => x!.Value);
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
                T? value = default;

                var dueTimeDisposable = new SerialDisposable();

                void internalCallback()
                {
                    if (hasValue)
                    {
                        // We have another value that came in to fire.
                        // Reregister for callback
                        dueTimeDisposable.Disposable = scheduler.Schedule(interval, internalCallback);
                        o.OnNext(value!);
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
            return Observable.Defer(() =>
            {
                return Observable.CombineLatest(
                    Observable.Interval(notificationFrequency, scheduler),
                    Observable.Return(DateTime.Now),
                    (_, startTime) =>
                    {
                        return DateTime.Now - startTime;
                    });
            });
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
                    return Observable.Return((false, default(TRet)!));
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
                            return Observable.Return((false, default(TRet)!));
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
            where TResource : class, IDisposable
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
            where TResource : class, IDisposable
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

        /// <summary>
        /// An observable that fires a signal whenever a specific file is created/modified/deleted.
        /// </summary>
        /// <param name="path">File path to watch</param>
        /// <param name="throwIfInvalidPath">Whether to error if path is invalid</param>
        /// <exception cref="ArgumentException">Will throw if file path has no parent directory, or is malformed, and the throw parameter is on</exception>
        /// <returns>Observable signal of when file created/modified/deleted</returns>
        public static IObservable<Unit> WatchFile(
            FilePath path, 
            bool throwIfInvalidPath = false,
            IFileSystemWatcherFactory? fileWatcherFactory = null)
        {
            fileWatcherFactory ??= new FileSystemWatcherFactory();
            return ObservableExt.UsingWithCatch(
                () =>
                {
                    var targetPath = Path.GetDirectoryName(path)!;
                    var watcher = fileWatcherFactory.CreateNew(targetPath, filter: Path.GetFileName(path));
                    watcher.EnableRaisingEvents = true;
                    return watcher;
                },
                (watcher) =>
                {
                    if (watcher.Failed)
                    {
                        if (throwIfInvalidPath)
                        {
                            throw watcher.Exception!;
                        }
                        else
                        {
                            return Observable.Empty<Unit>();
                        }
                    }
                    return Observable.Merge(
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Changed += h, h => watcher.Value.Changed -= h),
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Created += h, h => watcher.Value.Created -= h),
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Deleted += h, h => watcher.Value.Deleted -= h))
                        .Where(x => x.EventArgs.FullPath.Equals(path.Path, StringComparison.OrdinalIgnoreCase))
                        .Unit();
                })
                .Replay(1)
                .RefCount();
        }

        /// <summary>
        /// An observable that fires a signal whenever a specific file is created/modified/deleted.
        /// </summary>
        /// <param name="path">File path to watch</param>
        /// <param name="throwIfInvalidPath">Whether to error if path is invalid</param>
        /// <exception cref="ArgumentException">Will throw if file path has no parent directory, or is malformed, and the throw parameter is on</exception>
        /// <returns>Observable signal of when file created/modified/deleted</returns>
        public static IObservable<Unit> WatchFolder(
            DirectoryPath path, 
            bool throwIfInvalidPath = false,
            IFileSystemWatcherFactory? fileWatcherFactory = null)
        {
            fileWatcherFactory ??= new FileSystemWatcherFactory();
            return ObservableExt.UsingWithCatch(
                () =>
                {
                    var watcher = fileWatcherFactory.CreateNew(path);
                    watcher.EnableRaisingEvents = true;
                    return watcher;
                },
                (watcher) =>
                {
                    if (watcher.Failed)
                    {
                        if (throwIfInvalidPath)
                        {
                            throw watcher.Exception!;
                        }
                        else
                        {
                            return Observable.Empty<Unit>();
                        }
                    }
                    return Observable.Merge(
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Changed += h, h => watcher.Value.Changed -= h),
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Created += h, h => watcher.Value.Created -= h),
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Deleted += h, h => watcher.Value.Deleted -= h))
                        .Where(x => x.EventArgs.FullPath.Equals(path.Path, StringComparison.OrdinalIgnoreCase))
                        .Unit();
                })
                .Replay(1)
                .RefCount();
        }

        /// <summary>
        /// An observable stream of a folder's contents
        /// </summary>
        /// <param name="path">File path to watch</param>
        /// <param name="throwIfInvalidPath">Whether to error if path is invalid</param>
        /// <param name="fileSystem">FIlesystem to watch</param>
        /// <exception cref="ArgumentException">Will throw if file path has no parent directory, or is malformed, and the throw parameter is on</exception>
        /// <returns>Observable signal of when file created/modified/deleted</returns>
        public static IObservable<ChangeSet<FilePath, FilePath>> WatchFolderContents(
            DirectoryPath path,
            bool throwIfInvalidPath = false,
            IFileSystem? fileSystem = null)
        {
            fileSystem ??= IFileSystemExt.DefaultFilesystem;
            return ObservableExt.UsingWithCatch(
                () =>
                {
                    var watcher = fileSystem.FileSystemWatcher.CreateNew(path);
                    watcher.EnableRaisingEvents = true;
                    return watcher;
                },
                (watcher) =>
                {
                    if (watcher.Failed)
                    {
                        if (throwIfInvalidPath)
                        {
                            throw watcher.Exception!;
                        }
                        else
                        {
                            return Observable.Empty<ChangeSet<FilePath, FilePath>>();
                        }
                    }
                    return Observable.Merge(
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Created += h, h => watcher.Value.Created -= h),
                            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => watcher.Value.Deleted += h, h => watcher.Value.Deleted -= h))
                        .Select(x =>
                        {
                            switch (x.EventArgs.ChangeType)
                            {
                                case WatcherChangeTypes.Created:
                                    return new ChangeSet<FilePath, FilePath>(new Change<FilePath, FilePath>(ChangeReason.Add, key: x.EventArgs.FullPath, current: x.EventArgs.FullPath).AsEnumerable());
                                case WatcherChangeTypes.Deleted:
                                    return new ChangeSet<FilePath, FilePath>(new Change<FilePath, FilePath>(ChangeReason.Remove, key: x.EventArgs.FullPath, current: x.EventArgs.FullPath).AsEnumerable());
                                default:
                                    throw new NotImplementedException();
                            }
                        })
                        .Merge(Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => watcher.Value.Renamed += h, h => watcher.Value.Renamed -= h)
                            .Select(x =>
                            {
                                return new ChangeSet<FilePath, FilePath>(new Change<FilePath, FilePath>(ChangeReason.Moved, key: x.EventArgs.FullPath, current: x.EventArgs.FullPath, previous: new FilePath(x.EventArgs.OldFullPath)).AsEnumerable());
                            }))
                        .StartWith(
                            new ChangeSet<FilePath, FilePath>(fileSystem.Directory.EnumerateFiles(path)
                                .Select(x => new Change<FilePath, FilePath>(ChangeReason.Add, x, x))));
                });
        }

        /// These snippets were provided by RolandPheasant (author of DynamicData)
        /// They'll be going into the official library at some point, but are here for now.
        
        #region Dynamic Data EnsureUniqueChanges
        /// <summary>
        /// Removes outdated key events from a changeset, only leaving the last relevent change for each key.
        /// </summary>
        public static IObservable<IChangeSet<TObject, TKey>> EnsureUniqueChanges<TObject, TKey>(this IObservable<IChangeSet<TObject, TKey>> source)
            where TKey : notnull
        {
            return source.Select(EnsureUniqueChanges);
        }

        /// <summary>
        /// Removes outdated key events from a changeset, only leaving the last relevent change for each key.
        /// </summary>
        public static IChangeSet<TObject, TKey> EnsureUniqueChanges<TObject, TKey>(this IChangeSet<TObject, TKey> input)
            where TKey : notnull
        {
            var changes = input
                .GroupBy(kvp => kvp.Key)
                .Select(g => g.Aggregate(Optional<Change<TObject, TKey>>.None, Reduce))
                .Where(x => x.HasValue)
                .Select(x => x.Value);

            return new ChangeSet<TObject, TKey>(changes);
        }

        internal static Optional<Change<TObject, TKey>> Reduce<TObject, TKey>(Optional<Change<TObject, TKey>> previous, Change<TObject, TKey> next)
            where TKey : notnull
        {
            if (!previous.HasValue)
            {
                return next;
            }

            var previousValue = previous.Value;

            switch (previousValue.Reason)
            {
                case ChangeReason.Add when next.Reason == ChangeReason.Remove:
                    return Optional<Change<TObject, TKey>>.None;

                case ChangeReason.Remove when next.Reason == ChangeReason.Add:
                    return new Change<TObject, TKey>(ChangeReason.Update, next.Key, next.Current, previousValue.Current, next.CurrentIndex, previousValue.CurrentIndex);

                case ChangeReason.Add when next.Reason == ChangeReason.Update:
                    return new Change<TObject, TKey>(ChangeReason.Add, next.Key, next.Current, next.CurrentIndex);

                case ChangeReason.Update when next.Reason == ChangeReason.Update:
                    return new Change<TObject, TKey>(ChangeReason.Update, previousValue.Key, next.Current, previousValue.Previous, next.CurrentIndex, previousValue.PreviousIndex);

                default:
                    return next;
            }
        }
        #endregion

        public static IObservable<T> ObserveOnIfApplicable<T>(this IObservable<T> obs, IScheduler? scheduler)
        {
            return scheduler == null ? obs : obs.ObserveOn(scheduler);
        }

        public static IObservable<T> ThrottleWithOptionalScheduler<T>(this IObservable<T> obs, TimeSpan dueTime, IScheduler? scheduler)
        {
            return scheduler == null ? obs.Throttle(dueTime) : obs.Throttle(dueTime, scheduler);
        }

        public static IObservable<bool> Any(params IObservable<bool>[] observables)
        {
            return Observable.CombineLatest(observables)
                .Select(l => l.Any(x => x));
        }

        public static IObservable<bool> All(params IObservable<bool>[] observables)
        {
            return Observable.CombineLatest(observables)
                .Select(l => l.All(x => x));
        }

        public static IObservable<IChangeSet<TRet, TKey>> WhereCastable<TObj, TKey, TRet>(this IObservable<IChangeSet<TObj, TKey>> obs)
            where TKey : notnull
            where TRet : class, TObj
        {
            return obs.Filter(x => x is TRet)
                .Transform(x => (TRet)x!);
        }

        public static IObservable<IChangeSet<TRet>> WhereCastable<TObj, TRet>(this IObservable<IChangeSet<TObj>> obs)
            where TRet : class, TObj
        {
            return obs.Filter(x => x is TRet)
                .Transform(x => (TRet)x!);
        }

        public static IObservable<IChangeSet<TObj, TKey>> ChangeNotNull<TObj, TKey>(this IObservable<IChangeSet<TObj?, TKey>> obs)
            where TKey : notnull
            where TObj : class
        {
            return obs.Filter(x => x != null)!;
        }

        public static IObservable<IChangeSet<TObj>> ChangeNotNull<TObj>(this IObservable<IChangeSet<TObj?>> obs)
            where TObj : class
        {
            return obs.Filter(x => x != null)!;
        }
        
        public static IObservable<TSource> RetryWithBackOff<TSource, TException>(
            this IObservable<TSource> source,
            Func<TException, int, TimeSpan?> backOffStrategy,
            IScheduler scheduler)
            where TException : Exception
        {
            IObservable<TSource> Retry(int failureCount) =>
                source.Catch<TSource, TException>(
                    error =>
                    {
                        TimeSpan? delay = backOffStrategy(error, failureCount);
                        if (!delay.HasValue)
                        {
                            return Observable.Throw<TSource>(error);
                        }

                        return Observable.Timer(delay.Value, scheduler)
                            .SelectMany(Retry(failureCount + 1));
                    });

            return Retry(0);
        }

#if NETSTANDARD2_0
#else
        public static IObservable<TSource> RetryWithRampingBackoff<TSource, TException>(
            this IObservable<TSource> obs,
            TimeSpan interval, 
            TimeSpan max,
            IScheduler? scheduler = null)
            where TException : Exception
        {
            var maxTimes = max / interval;
            Func<TException, int, TimeSpan?> strat = (_, times) =>
            {
                if (times >= maxTimes)
                {
                    return max;
                }

                return times * interval;
            };
            if (scheduler == null)
            {
                return obs.RetryWithBackOff(strat);
            }
            else
            {
                return obs.RetryWithBackOff(strat, scheduler);
            }
        }
#endif

        public static IObservable<TSource> TakeUntilDisposed<TSource>(this IObservable<TSource> obs,
            CompositeDisposable disposable)
        {
            return obs.TakeUntil<TSource, Unit>(Observable.Create<Unit>(obs =>
            {
                var signal = Disposable.Create(() => obs.OnNext(System.Reactive.Unit.Default));
                disposable.Add(signal);
                return Disposable.Create(() => disposable.Remove(signal));
            }));
        }
    }
}
