using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Noggog;

public readonly struct MemorySlice<T> : IEnumerable<T>
{
    private readonly T[] _arr;
    private readonly int _startPos;
    private readonly int _length;
    public int Length => _length;
    public int StartPosition => _startPos;

    [DebuggerStepThrough]
    public MemorySlice(T[] arr)
    {
        _arr = arr;
        _startPos = 0;
        _length = arr.Length;
    }

    [DebuggerStepThrough]
    public MemorySlice(T[] arr, int startPos, int length)
    {
        _arr = arr;
        _startPos = startPos;
        _length = length;
    }

    [Pure]
    public Span<T> Span => _arr.AsSpan(start: _startPos, length: _length);

    public T this[int index]
    {
        get => _arr[index + _startPos];
        set => _arr[index + _startPos] = value;
    }

    [Pure]
    [DebuggerStepThrough]
    public MemorySlice<T> Slice(int start)
    {
        var startPos = _startPos + start;
        if (startPos < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        return new MemorySlice<T>(_arr, startPos, _length - start);
    }

    [Pure]
    [DebuggerStepThrough]
    public MemorySlice<T> Slice(int start, int length)
    {
        var startPos = _startPos + start;
        if (startPos < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (startPos + length > _arr.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        return new MemorySlice<T>(_arr, startPos, length);
    }

    [Pure]
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _length; i++)
        {
            yield return _arr[i + _startPos];
        }
    }

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Pure]
    public T[] ToArray()
    {
        if (_arr == null || _length == 0) return [];
        var ret = new T[_length];
        Array.Copy(_arr, _startPos, ret, 0, _length);
        return ret;
    }

    [Pure]
    public static implicit operator ReadOnlyMemorySlice<T>(MemorySlice<T> mem)
    {
        return new ReadOnlyMemorySlice<T>(
            mem._arr,
            mem._startPos,
            mem._length);
    }

    [Pure]
    public static implicit operator ReadOnlySpan<T>(MemorySlice<T> mem)
    {
        return mem.Span;
    }

    [Pure]
    public static implicit operator Span<T>(MemorySlice<T> mem)
    {
        return mem.Span;
    }

    [Pure]
    public static implicit operator MemorySlice<T>(T[] mem)
    {
        return new MemorySlice<T>(mem);
    }

    [Pure]
    public static implicit operator MemorySlice<T>?(T[]? mem)
    {
        if (mem == null) return null;
        return new MemorySlice<T>(mem);
    }
}

public readonly struct ReadOnlyMemorySlice<T> : IEnumerable<T>
{
    private readonly T[] _arr;
    private readonly int _startPos;
    private readonly int _length;
    public int Length => _length;
    public int StartPosition => _startPos;

    public static readonly ReadOnlyMemorySlice<T> Empty = new ReadOnlyMemorySlice<T>([]);

    [DebuggerStepThrough]
    public ReadOnlyMemorySlice(T[] arr)
    {
        _arr = arr;
        _startPos = 0;
        _length = arr.Length;
    }

    [DebuggerStepThrough]
    public ReadOnlyMemorySlice(T[] arr, int startPos, int length)
    {
        _arr = arr;
        _startPos = startPos;
        _length = length;
    }

    [Pure]
    public ReadOnlySpan<T> Span => _arr.AsSpan(start: _startPos, length: _length);

    [Pure]
    public T this[int index] => _arr[index + _startPos];

    [Pure]
    [DebuggerStepThrough]
    public ReadOnlyMemorySlice<T> Slice(int start)
    {
        var startPos = _startPos + start;
        if (startPos < 0)
        {
            throw new ArgumentOutOfRangeException();
        }

        return new ReadOnlyMemorySlice<T>(_arr, _startPos + start, _length - start);
    }

    [Pure]
    [DebuggerStepThrough]
    public ReadOnlyMemorySlice<T> SliceUpTo(int maxLength)
    {
        return Slice(0, Math.Min(_length, maxLength));
    }

    [Pure]
    [DebuggerStepThrough]
    public ReadOnlyMemorySlice<T> Slice(int start, int length)
    {
        var startPos = _startPos + start;
        if (startPos < 0)
        {
            throw new ArgumentOutOfRangeException();
        }
        if (startPos + length > _arr.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        return new ReadOnlyMemorySlice<T>(_arr, _startPos + start, length);
    }

    [Pure]
    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _length; i++)
        {
            yield return _arr[i + _startPos];
        }
    }

    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    [Pure]
    public T[] ToArray()
    {
        if (_arr == null || _length == 0) return [];
        var ret = new T[_length];
        Array.Copy(_arr, _startPos, ret, 0, _length);
        return ret;
    }

    [Pure]
    public static implicit operator ReadOnlySpan<T>(ReadOnlyMemorySlice<T> mem)
    {
        return mem.Span;
    }

    [Pure]
    public static implicit operator ReadOnlyMemorySlice<T>?(T[]? mem)
    {
        if (mem == null) return null;
        return new ReadOnlyMemorySlice<T>(mem);
    }

    [Pure]
    public static implicit operator ReadOnlyMemorySlice<T>(T[] mem)
    {
        return new ReadOnlyMemorySlice<T>(mem);
    }
}

public static class MemorySliceExt
{
    public static bool SequenceEqual<T>(this ReadOnlyMemorySlice<T>? lhs, ReadOnlyMemorySlice<T>? rhs)
        where T : IEquatable<T>
    {
        if (lhs == null && rhs == null) return true;
        if (lhs == null || rhs == null) return false;
        return MemoryExtensions.SequenceEqual(lhs.Value.Span, rhs.Value.Span);
    }
}