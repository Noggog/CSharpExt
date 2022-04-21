using System.Collections;

namespace Noggog;

public struct RangeInt8 : IEquatable<RangeInt8>, IEnumerable<sbyte>
{
    private sbyte _min;
    public sbyte Min
    {
        get => _min;
        set => _min = value;
    }

    private sbyte _max;
    public sbyte Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((Max - Min) / 2f) + Min;
    public sbyte Difference => (sbyte)(Max - Min);
    public ushort Width => (ushort)(Max - Min + 1);

    public RangeInt8(sbyte val1, sbyte val2)
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

    public RangeInt8(sbyte? min, sbyte? max)
        : this(min ?? sbyte.MinValue, max ?? sbyte.MaxValue)
    {
    }

    public RangeInt8(sbyte val)
        : this(val, val)
    {
    }

    public static RangeInt8 FromLength(sbyte loc, sbyte length)
    {
        return new RangeInt8(
            loc,
            (sbyte)(loc + length - 1));
    }

    public static RangeInt8 Parse(string str)
    {
        if (!TryParse(str, out RangeInt8 rd))
        {
            return default(RangeInt8);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeInt8 rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeInt8);
            return false;
        }
        rd = new RangeInt8(
            sbyte.Parse(split[0]),
            sbyte.Parse(split[1]));
        return true;
    }

    public bool IsInRange(sbyte i)
    {
        if (i > _max) return false;
        if (i < _min) return false;
        return true;
    }

    public sbyte PutInRange(sbyte f, bool throwException = true)
    {
        if (throwException)
        {
            if (f < _min)
            {
                throw new ArgumentException($"_min is out of range: {f} < {_min}");
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

    public bool IsInRange(RangeInt8 r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeInt8 PutInRange(RangeInt8 r, bool throwException = true)
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
            sbyte min = r._min < _min ? _min : r._min;
            sbyte max = r._max < _max ? _max : r._max;
            return new RangeInt8(min, max);
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not RangeInt8 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeInt8 other)
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

    public static bool operator ==(RangeInt8 c1, RangeInt8 c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeInt8 c1, RangeInt8 c2)
    {
        return !c1.Equals(c2);
    }

    public IEnumerator<sbyte> GetEnumerator()
    {
        for (sbyte i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}