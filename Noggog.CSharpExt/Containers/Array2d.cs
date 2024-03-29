using System.Collections;

namespace Noggog;

public class Array2d<T> : IArray2d<T>, IShallowCloneable<Array2d<T>>
{
    private readonly T[,] _arr;

    public int Width => _arr.GetLength(0);
    public int Height => _arr.GetLength(1);

    public Array2d(T[,] arr)
    {
        _arr = arr;
    }

    public Array2d(IReadOnlyArray2d<T> rhs)
    {
        _arr = new T[rhs.Width, rhs.Height];
        for (int y = 0; y < rhs.Height; y++)
        {
            for (int x = 0; x < rhs.Width; x++)
            {
                this[x, y] = rhs[x, y];
            }
        }
    }

    public Array2d(int width, int height, T initialVal)
    {
        _arr = new T[width, height];
        SetAllTo(initialVal);
    }

    public Array2d(int width, int height, Func<T> initialVal)
    {
        _arr = new T[width, height];
        SetAllTo(initialVal);
    }

    public Array2d(P2Int size, T initialVal)
        : this(size.X, size.Y, initialVal)
    {
    }

    public T this[int xIndex, int yIndex]
    {
        get => _arr[xIndex, yIndex];
        set => _arr[xIndex, yIndex] = value;
    }

    public T this[P2Int index]
    {
        get => _arr[index.X, index.Y];
        set => _arr[index.X, index.Y] = value;
    }

    public void SetTo(IReadOnlyArray2d<T> rhs)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                this[x, y] = rhs[x, y];
            }
        }
    }

    public void Set(IEnumerable<KeyValuePair<P2Int, T>> vals)
    {
        foreach (var pt in vals)
        {
            this[pt.Key] = pt.Value;
        }
    }

    public void SetAllTo(T item)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _arr[x, y] = item;
            }
        }
    }

    public void SetAllTo(Func<T> item)
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                _arr[x, y] = item();
            }
        }
    }

    public IEnumerator<KeyValuePair<P2Int, T>> GetEnumerator()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return new KeyValuePair<P2Int, T>(new P2Int(x, y), _arr[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Array2d<T> ShallowClone()
    {
        return new Array2d<T>((_arr.Clone() as T[,])!);
    }

    object IShallowCloneable.ShallowClone() => ShallowClone();

    IEnumerator<IKeyValue<P2Int, T>> IEnumerable<IKeyValue<P2Int, T>>.GetEnumerator()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                yield return new KeyValue<P2Int, T>(new P2Int(x, y), _arr[x, y]);
            }
        }
    }
}