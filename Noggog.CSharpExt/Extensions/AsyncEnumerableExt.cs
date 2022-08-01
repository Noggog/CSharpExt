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
}