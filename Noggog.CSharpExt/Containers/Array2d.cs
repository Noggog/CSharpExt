using System;
using System.Collections;
using System.Collections.Generic;

namespace Noggog;

public class Array2d<T> : IArray2d<T>, IShallowCloneable<Array2d<T>>
{
    public static readonly IReadOnlyArray2d<T> Empty = new Array2d<T>(0, 0);

    private readonly T[,] _arr;

    public int Width => _arr.GetLength(0);
    public int Height => _arr.GetLength(1);

    public Array2d(T[,] arr)
    {
        _arr = arr;
    }

    public Array2d(int width, int height)
    {
        _arr = new T[width, height];
    }

    public Array2d(P2Int size)
        : this(size.X, size.Y)
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

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public Array2d<T> ShallowClone()
    {
        return new Array2d<T>((_arr.Clone() as T[,])!);
    }

    object IShallowCloneable.ShallowClone() => this.ShallowClone();

    IArray2d<T> IShallowCloneable<IArray2d<T>>.ShallowClone() => this.ShallowClone();
}