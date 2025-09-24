using System.Diagnostics.Contracts;

namespace Noggog;

public static class ArrayExt
{
    [Pure]
    public static bool Contains<T>(this T[] arr, T val)
    {
        foreach (T t in arr)
        {
            if (t == null) continue;
            if (t.Equals(val))
            {
                return true;
            }
        }
        return false;
    }

    [Pure]
    public static P2Int Center<T>(this T[,] array)
    {
        return new P2Int(array.GetLength(0) / 2, array.GetLength(1) / 2);
    }

    [Pure]
    public static bool InRange<T>(this T[,] array, int x, int y)
    {
        return x >= 0 
               && y >= 0 
               && y < array.GetLength(0) 
               && x < array.GetLength(1);
    }

    public static void Fill<T>(this T[] array, T to)
    {
        for (int x = 0; x < array.Length; x++)
        {
            array[x] = to;
        }
    }

    public static void Fill<T>(this T[,] array, T to)
    {
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                array[y, x] = to;
            }
        }
    }

    public static void Fill<T>(this T[][] array, T to)
    {
        for (int y = 0; y < array.Length; y++)
        {
            T[] arrx = array[y];
            for (int x = 0; x < arrx.Length; x++)
            {
                arrx[x] = to;
            }
        }
    }

    public static void Fill<T>(this T[] array, Func<T> to)
    {
        for (int x = 0; x < array.Length; x++)
        {
            array[x] = to();
        }
    }

    [Pure]
    public static T[] Create<T>(int size, Func<int, T> factory)
    {
        T[] ret = new T[size];
        for (int x = 0; x < ret.Length; x++)
        {
            ret[x] = factory(x);
        }
        return ret;
    }
    
    [Pure]
    public static T[] Create<T>(int size, T val)
    {
        var ret = new T[size];
        ret.Fill(val);
        return ret;
    }
    
    [Pure]
    public static T[,] Copy<T>(this T[,] array)
    {
        T[,] ret = new T[array.GetLength(0), array.GetLength(1)];
        Array.Copy(array, ret, array.Length);
        return ret;
    }

    [Pure]
    public static T[,] Expand<T>(this T[,] array, int buffer)
    {
        T[,] ret = new T[array.GetLength(0) + 2 * buffer, array.GetLength(1) + 2 * buffer];
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                ret[y + buffer, x + buffer] = array[y, x];
            }
        }
        return ret;
    }

    [Pure]
    public static T[] ToArray<T>(this T[] arr, int validCount)
    {
        T[] ret = new T[validCount];
        for (int i = 0; i < validCount; i++)
        {
            ret[i] = arr[i];
        }
        return ret;
    }

    [Pure]
    public static IEnumerable<(T Item, bool Last)> IterateMarkLast<T>(this T[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            yield return (arr[i], i == arr.Length - 1);
        }
    }

    [Pure]
    public static bool InRange<T>(this T[] arr, int index)
    {
        return index >= 0 && index < arr.Length;
    }

    public static void Reset<T>(this T[] arr)
        where T : struct
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = default;
        }
    }

    public static void ResetToNull<T>(this T?[] arr)
        where T : class
    {
        for (int i = 0; i < arr.Length; i++)
        {
            arr[i] = default;
        }
    }
}

public static class ArrayExt<T>
{
    public static readonly T[] Empty = new T[0];
}