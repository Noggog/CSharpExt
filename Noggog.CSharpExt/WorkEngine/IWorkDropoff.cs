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
    Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, TRet> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, TRet> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, Task> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, Task<TRet>> action, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, Task<TRet>> action, CancellationToken cancellationToken = default);
}
#endif