using System.Collections;

namespace Noggog;

public struct RangeInt32 : IEquatable<RangeInt32>, IEnumerable<int>
{
    private int _min;
    public int Min
    {
        get => _min;
        set => _min = value;
    }

    private int _max;
    public int Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((_max - _min) / 2f) + _min;
    public int Difference => _max - _min;
    public int Width => _max - _min + 1;
    public int IntWidth => _max - _min + 1;

    public RangeInt32(int val1, int val2)
    {
        if (val1 > val2)
        {
            _max = val1;
            _min = val2;
        }
        else
        {
            _min = val1;
            _max = val2;
        }
    }

    public RangeInt32(int? min, int? max)
        : this(min ?? int.MinValue, max ?? int.MaxValue)
    {
    }

    public RangeInt32(int val)
        : this(val, val)
    {
    }

    public static RangeInt32 FromLength(int loc, int length)
    {
        return new RangeInt32(
            loc,
            loc + length - 1);
    }

    public static RangeInt32 Parse(string str)
    {
        if (!TryParse(str, out RangeInt32 rd))
        {
            return default(RangeInt32);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeInt32 rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeInt32);
            return false;
        }
        rd = new RangeInt32(
            int.Parse(split[0]),
            int.Parse(split[1]));
        return true;
    }

    public bool IsInRange(int i)
    {
        if (i > _max) return false;
        if (i < _min) return false;
        return true;
    }

    public int PutInRange(int f, bool throwException = true)
    {
        if (throwException)
        {
            if (f < _min)
            {
                throw new ArgumentException($"Min is out of range: {f} < {_min}");
            }
            if (f > _max)
            {
                throw new ArgumentException($"Max is out of range: {f} < {_max}");
            }
        }
        else
        {
            if (f > _max) return _max;
            if (f < _min) return _min;
        }
        return f;
    }

    public bool IsInRange(RangeInt32 r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeInt32 PutInRange(RangeInt32 r, bool throwException = true)
    {
        if (throwException)
        {
            if (r._min < _min)
            {
                throw new ArgumentException($"Min is out of range: {r._min} < {_min}");
            }
            if (r._max > _max)
            {
                throw new ArgumentException($"Max is out of range: {r.Max} < {Max}");
            }
            return r;
        }
        else
        {
            int min = r._min < _min ? _min : r._min;
            int max = r._max < _max ? _max : r._max;
            return new RangeInt32(min, max);
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not RangeInt32 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeInt32 other)
    {
        return _min == other._min
               && _max == other._max;
    }

    public override int GetHashCode() => HashCode.Combine(_min, _max);

    public override string ToString()
    {
        return _min == _max ? $"({_min})" : $"({_min} - {_max})";
    }

    public string ToString(string format)
    {
        string prefix;
        if (format.Contains("X"))
        {
            prefix = "0x";
        }
        else
        {
            prefix = string.Empty;
        }
        return _min == _max ? $"({prefix}{_min.ToString(format)})" : $"({prefix}{_min.ToString(format)} - {prefix}{_max.ToString(format)})";
    }

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static bool operator ==(RangeInt32 c1, RangeInt32 c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeInt32 c1, RangeInt32 c2)
    {
        return !c1.Equals(c2);
    }
}