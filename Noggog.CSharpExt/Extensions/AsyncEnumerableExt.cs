namespace Noggog;

using System.Linq;

public static class AsyncEnumerableExt
{
    public static IAsyncEnumerable<T> NotNull<T>(this IAsyncEnumerable<T?> e)
        where T : class
    {
        return e.Where(i => i != null)!;
    }

    public static IAsyncEnumerable<T> NotNull<T>(this IAsyncEnumerable<T?> e)
        where T : struct
    {
        return e.Where(i => i.HasValue)
            .Select(i => i!.Value);
    }

    public static IAsyncEnumerable<T> DoAwait<T>(this IAsyncEnumerable<T> e, Func<T, Task> doJob)
    {
#if NET10_0_OR_GREATER
        return DoAwaitImpl(e, doJob);
#else
        return e.SelectAwait(async i =>
        {
            await doJob(i);
            return i;
        });
#endif
    }

#if NET10_0_OR_GREATER
    private static async IAsyncEnumerable<T> DoAwaitImpl<T>(IAsyncEnumerable<T> e, Func<T, Task> doJob)
    {
        await foreach (var item in e)
        {
            await doJob(item);
            yield return item;
        }
    }
#endif
}