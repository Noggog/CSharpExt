using System.Collections;

namespace Noggog;

public struct RangeUInt64 : IEquatable<RangeUInt64>, IEnumerable<ulong>
{
    private ulong _min;
    public ulong Min
    {
        get => _min;
        set => _min = value;
    }

    private ulong _max;
    public ulong Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((_max - _min) / 2f) + _min;
    public ulong Difference => (ulong)(_max - _min);
    public ulong Width => (ulong)(_max - _min + 1);

    public RangeUInt64(ulong val1, ulong val2)
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

    public RangeUInt64(ulong? min, ulong? max)
        : this(min ?? ulong.MinValue, max ?? ulong.MaxValue)
    {
    }

    public RangeUInt64(ulong val)
        : this(val, val)
    {
    }

    public static RangeUInt64 FromLength(ulong loc, ulong length)
    {
        return new RangeUInt64(
            loc,
            loc + length - 1);
    }

    public static RangeUInt64 Parse(string str)
    {
        if (!TryParse(str, out RangeUInt64 rd))
        {
            return default(RangeUInt64);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeUInt64 rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeUInt64);
            return false;
        }
        rd = new RangeUInt64(
            ulong.Parse(split[0]),
            ulong.Parse(split[1]));
        return true;
    }

    public bool IsInRange(ulong i)
    {
        if (i > _max) return false;
        if (i < _min) return false;
        return true;
    }

    public ulong PutInRange(ulong f, bool throwException = true)
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

    public bool IsInRange(RangeUInt64 r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeUInt64 PutInRange(RangeUInt64 r, bool throwException = true)
    {
        if (throwException)
        {
            if (r._min < _min)
            {
                throw new ArgumentException($"Min is out of range: {r._min} < {_min}");
            }
            if (r._max > _max)
            {
                throw new ArgumentException($"Max is out of range: {r._max} < {_max}");
            }
            return r;
        }
        else
        {
            ulong min = r._min < _min ? _min : r._min;
            ulong max = r._max < _max ? _max : r._max;
            return new RangeUInt64(min, max);
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not RangeUInt64 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeUInt64 other)
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

    public static bool operator ==(RangeUInt64 c1, RangeUInt64 c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeUInt64 c1, RangeUInt64 c2)
    {
        return !c1.Equals(c2);
    }

    public IEnumerator<ulong> GetEnumerator()
    {
        for (ulong i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}