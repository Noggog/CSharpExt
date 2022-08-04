#if NETSTANDARD2_0 
#else
namespace Noggog.WorkEngine;
public interface IWorkDropoff
{
    Task Enqueue(Action toDo, CancellationToken cancellationToken = default);
    Task Enqueue(Func<Task> toDo, CancellationToken cancellationToken = default);
    Task EnqueueAndWait(Action toDo, CancellationToken cancellationToken = default);
    Task<T> EnqueueAndWait<T>(Func<T> toDo, CancellationToken cancellationToken = default);
    Task EnqueueAndWait(Func<Task> toDo, CancellationToken cancellationToken = default);
    Task<T> EnqueueAndWait<T>(Func<Task<T>> toDo, CancellationToken cancellationToken = default);
}

public static class IWorkDropoffExt
{
    public static async Task<T[]> EnqueueAndWait<T>(this IWorkDropoff dropoff, IEnumerable<T> items, Action<T> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(() =>
            {
                action(x);
                return x;
            }, cancellationToken)));
    }
    
    public static async Task<T[]> EnqueueAndWait<T>(this IWorkDropoff dropoff, IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(() =>
            {
                action(x, cancellationToken);
                return x;
            }, cancellationToken)));
    }
    
    public static async Task<TRet[]> EnqueueAndWait<TIn, TRet>(this IWorkDropoff dropoff, IEnumerable<TIn> items, Func<TIn, TRet> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(() =>
            {
                return action(x);
            }, cancellationToken)));
    }
    
    public static async Task<TRet[]> EnqueueAndWait<TIn, TRet>(this IWorkDropoff dropoff, IEnumerable<TIn> items, Func<TIn, CancellationToken, TRet> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(() =>
            {
                return action(x, cancellationToken);
            }, cancellationToken)));
    }

    public static async Task<T[]> EnqueueAndWait<T>(this IWorkDropoff dropoff, IEnumerable<T> items, Func<T, Task> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(async () =>
            {
                await action(x);
                return x;
            }, cancellationToken)));
    }

    public static async Task<T[]> EnqueueAndWait<T>(this IWorkDropoff dropoff, IEnumerable<T> items, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(async () =>
            {
                await action(x, cancellationToken);
                return x;
            }, cancellationToken)));
    }

    public static async Task<TRet[]> EnqueueAndWait<TIn, TRet>(this IWorkDropoff dropoff, IEnumerable<TIn> items, Func<TIn, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(async () =>
            {
                return await action(x);
            }, cancellationToken)));
    }

    public static async Task<TRet[]> EnqueueAndWait<TIn, TRet>(this IWorkDropoff dropoff, IEnumerable<TIn> items, Func<TIn, CancellationToken, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => dropoff.EnqueueAndWait(async () =>
            {
                return await action(x, cancellationToken);
            }, cancellationToken)));
    }
}
#endif