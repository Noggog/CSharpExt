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
    new IEnumerator<KeyValuePair<P2Int, T>> GetEnumerator();
}

public static class IReadOnlyArray2d
{
    public static Array2d<T> ShallowClone<T>(this IReadOnlyArray2d<T> arr)
    {
        if (arr is Array2d<T> direct)
        {
            return direct.ShallowClone();
        }

        var ret = new Array2d<T>(arr.Height, arr.Width);
        for (int y = 0; y < arr.Height; y++)
        {
            for (int x = 0; x < arr.Width; x++)
            {
                ret[x, y] = arr[x, y];
            }
        }

        return ret;
    }
}