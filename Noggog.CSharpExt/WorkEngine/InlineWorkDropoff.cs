#if NETSTANDARD2_0 
#else
namespace Noggog.WorkEngine;

public class InlineWorkDropoff : IWorkDropoff
{
    public static readonly InlineWorkDropoff Instance = new();

    public async Task Enqueue(Action toDo, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        toDo();
    }

    public async Task Enqueue(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        await toDo().ConfigureAwait(false);
    }

    public async Task EnqueueAndWait(Action toDo, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        toDo();
    }

    public async Task<T> EnqueueAndWait<T>(Func<T> toDo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return toDo();
    }

    public async Task EnqueueAndWait(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        await toDo().ConfigureAwait(false);
    }

    public async Task<T> EnqueueAndWait<T>(Func<Task<T>> toDo, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await toDo().ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var arr = items.ToArray();
        arr.ForEach(action);
        return arr;
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var arr = items.ToArray();
        arr.ForEach(x => action(x, cancellationToken));
        return arr;
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, TRet> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return items.Select(x => action(x)).ToArray();
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, TRet> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return items.Select(x => action(x, cancellationToken)).ToArray();
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, Task> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var arr = items.ToArray();
        await Task.WhenAll(arr.Select(x => action(x))).ConfigureAwait(false);
        return arr;
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.WhenAll(items.Select(x => action(x, cancellationToken))).ConfigureAwait(false);
        return items.ToArray();
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.WhenAll(items.Select(x => action(x))).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await Task.WhenAll(items.Select(x => action(x, cancellationToken))).ConfigureAwait(false);
    }
}
#endif