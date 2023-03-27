namespace Noggog.Extensions;

public static class ParallelQueryExt
{
    public static IEnumerable<T> NotNull<T>(this ParallelQuery<T?> e)
        where T : class
    {
        return e.Where(i => i != null)!;
    }

    public static IEnumerable<T> NotNull<T>(this ParallelQuery<T?> e)
        where T : struct
    {
        return e.Where(i => i.HasValue)
            .Select(i => i!.Value);
    }
}