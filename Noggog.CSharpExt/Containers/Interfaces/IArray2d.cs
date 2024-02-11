namespace Noggog;

public interface IReadOnlyArray2d<out T> : IEnumerable<IKeyValue<P2Int, T>>
{
    int Width { get; }
    int Height { get; }
    T this[int xIndex, int yIndex] { get; }
    T this[P2Int index] { get; }
}

public interface IArray2d<T> : IReadOnlyArray2d<T>, IEnumerable<KeyValuePair<P2Int, T>>
{
    new T this[int xIndex, int yIndex] { get; set; }
    new T this[P2Int index] { get; set; }
    public void SetTo(IReadOnlyArray2d<T> rhs);
    public void Set(IEnumerable<KeyValuePair<P2Int, T>> vals);
    public void SetAllTo(T item);
    public void SetAllTo(Func<T> item);
    new IEnumerator<KeyValuePair<P2Int, T>> GetEnumerator();
}

public static class IArray2dExt
{
    public static Array2d<T> ShallowClone<T>(this IReadOnlyArray2d<T> arr)
    {
        if (arr is Array2d<T> direct)
        {
            return direct.ShallowClone();
        }

        return new Array2d<T>(arr);
    }
    
    public static void SetTo<T>(this IArray2d<T> arr, IEnumerable<KeyValuePair<P2Int, T>> vals, T fallback)
    {
        arr.SetAllTo(fallback);
        arr.Set(vals);
    }
    
    public static void SetTo<T>(this IArray2d<T> arr, IEnumerable<KeyValuePair<P2Int, T>> vals, Func<T> fallback)
    {
        arr.SetAllTo(fallback);
        arr.Set(vals);
    }
}