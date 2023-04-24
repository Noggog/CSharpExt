#if NETSTANDARD2_0 
#else
using System.Threading.Channels;

namespace Noggog.WorkEngine;

public class WorkDropoff : IWorkDropoff, IWorkQueue
{
    private readonly Channel<IToDo> _channel = Channel.CreateUnbounded<IToDo>();
    public ChannelReader<IToDo> Reader => _channel.Reader;

    private async Task ProcessExistingQueue()
    {
        if (WorkConsumer.AsyncLocalCurrentQueue.Value == null) return;
        while (WorkConsumer.AsyncLocalCurrentQueue.Value.Reader.TryRead(out var toDo))
        {
            if (toDo.IsAsync)
            {
                await toDo.DoAsync();
            }
            else
            {
                toDo.Do();
            }
        }
    }

    public async Task Enqueue(Action toDo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(new ToDo(toDo, null), cancellationToken).ConfigureAwait(false);
    }

    public async Task EnqueueAndWait(Action toDo, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource();
        await _channel.Writer.WriteAsync(new ToDo(toDo, tcs), cancellationToken).ConfigureAwait(false);
        await ProcessExistingQueue().ConfigureAwait(false);
        await tcs.Task.ConfigureAwait(false);
    }

    public async Task<T> EnqueueAndWait<T>(Func<T> toDo, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<T>();
        await _channel.Writer.WriteAsync(new ToDo<T>(toDo, tcs), cancellationToken).ConfigureAwait(false);
        await ProcessExistingQueue().ConfigureAwait(false);
        return await tcs.Task.ConfigureAwait(false);
    }

    public async Task Enqueue(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(new ToDo(toDo, null), cancellationToken).ConfigureAwait(false);
    }
        
    public async Task EnqueueAndWait(Func<Task> toDo, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource();
        await _channel.Writer.WriteAsync(new ToDo(toDo, tcs), cancellationToken).ConfigureAwait(false);
        await ProcessExistingQueue().ConfigureAwait(false);
        await tcs.Task.ConfigureAwait(false);
    }

    public async Task<T> EnqueueAndWait<T>(Func<Task<T>> toDo, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<T>();
        await _channel.Writer.WriteAsync(new ToDo<T>(toDo, tcs), cancellationToken).ConfigureAwait(false);
        await ProcessExistingQueue().ConfigureAwait(false);
        return await tcs.Task.ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(() =>
            {
                action(x);
                return x;
            }, cancellationToken)));
    }
    
    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Action<T, CancellationToken> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(() =>
            {
                action(x, cancellationToken);
                return x;
            }, cancellationToken)));
    }
    
    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, TRet> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(() =>
            {
                return action(x);
            }, cancellationToken)));
    }
    
    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, TRet> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(() =>
            {
                return action(x, cancellationToken);
            }, cancellationToken)));
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, Task> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(async () =>
            {
                await action(x);
                return x;
            }, cancellationToken)));
    }

    public async Task<IReadOnlyList<T>> EnqueueAndWait<T>(IEnumerable<T> items, Func<T, CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(async () =>
            {
                await action(x, cancellationToken);
                return x;
            }, cancellationToken)));
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(async () =>
            {
                return await action(x);
            }, cancellationToken)));
    }

    public async Task<IReadOnlyList<TRet>> EnqueueAndWait<TIn, TRet>(IEnumerable<TIn> items, Func<TIn, CancellationToken, Task<TRet>> action, CancellationToken cancellationToken = default)
    {
        return await Task.WhenAll(items
            .Select(x => EnqueueAndWait(async () =>
            {
                return await action(x, cancellationToken);
            }, cancellationToken)));
    }
}
#endif