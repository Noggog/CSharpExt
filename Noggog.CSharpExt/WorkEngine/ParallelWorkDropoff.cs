#if NETSTANDARD2_0 
#else
namespace Noggog.WorkEngine;

public class ParallelWorkDropoff : IWorkDropoff
{
    private readonly InlineWorkDropoff _inline = new();
    
    public async Task Enqueue(Action toDo, CancellationToken cancellationToken = default)
    {
        await _inline.Enqueue(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task Enqueue(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        await _inline.Enqueue(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task EnqueueAndWait(Action toDo, CancellationToken cancellationToken = default)
    {
        await _inline.Enqueue(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> EnqueueAndWait<T>(Func<T> toDo, CancellationToken cancellationToken = default)
    {
        return await _inline.EnqueueAndWait(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task EnqueueAndWait(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        await _inline.EnqueueAndWait(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task<T> EnqueueAndWait<T>(Func<Task<T>> toDo, CancellationToken cancellationToken = default)
    {
        return await _inline.EnqueueAndWait(toDo, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        Parallel.ForEach(arr, new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            (i, _) =>
            {
                action(i);
            });
        return arr;
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        Parallel.ForEach(arr, new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            (i, _) =>
            {
                action(i, cancellationToken);
            });
        return arr;
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, TRet> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        var ret = new TRet[arr.Length];
        Parallel.ForEach(arr.WithIndex(), new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            (i, s) =>
            {
                ret[i.Index] = action(i.Item);
            });
        return ret;
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, TRet> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        var ret = new TRet[arr.Length];
        Parallel.ForEach(arr.WithIndex(), new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            (i, s) =>
            {
                ret[i.Index] = action(i.Item, cancellationToken);
            });
        return ret;
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, Task> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        await Parallel.ForEachAsync(arr, new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            async (i, s) =>
            {
                await action(i).ConfigureAwait(false);
            }).ConfigureAwait(false);
        return arr;
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        await Parallel.ForEachAsync(arr, new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            async (i, s) =>
            {
                await action(i, cancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);
        return arr;
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        var ret = new TRet[arr.Length];
        await Parallel.ForEachAsync(arr.WithIndex(), new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            async (i, s) =>
            {
                ret[i.Index] = await action(i.Item).ConfigureAwait(false);
            }).ConfigureAwait(false);
        return ret;
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        var arr = items.ToArray();
        var ret = new TRet[arr.Length];
        await Parallel.ForEachAsync(arr.WithIndex(), new ParallelOptions()
            {
                CancellationToken = cancellationToken
            },
            async (i, s) =>
            {
                ret[i.Index] = await action(i.Item, cancellationToken).ConfigureAwait(false);
            }).ConfigureAwait(false);
        return ret;
    }
}
#endif