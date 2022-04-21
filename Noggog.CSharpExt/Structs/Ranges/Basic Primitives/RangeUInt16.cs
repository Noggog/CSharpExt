using System.Collections;

namespace Noggog;

public struct RangeUInt16 : IEquatable<RangeUInt16>, IEnumerable<ushort>
{
    private ushort _min;
    public ushort Min
    {
        get => _min;
        set => _min = value;
    }

    private ushort _max;
    public ushort Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((Max - Min) / 2f) + Min;
    public ushort Difference => (ushort)(Max - Min);
    public uint Width => (uint)(Max - Min + 1);

    public RangeUInt16(ushort val1, ushort val2)
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

    public RangeUInt16(ushort? min, ushort? max)
        : this(min ?? ushort.MinValue, max ?? ushort.MaxValue)
    {
    }

    public RangeUInt16(ushort val)
        : this(val, val)
    {
    }

    public static RangeUInt16 FromLength(ushort loc, ushort length)
    {
        return new RangeUInt16(
            loc,
            (ushort)(loc + length - 1));
    }

    public static RangeUInt16 Parse(string str)
    {
        if (!TryParse(str, out RangeUInt16 rd))
        {
            return default(RangeUInt16);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeUInt16 rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeUInt16);
            return false;
        }
        rd = new RangeUInt16(
            ushort.Parse(split[0]),
            ushort.Parse(split[1]));
        return true;
    }

    public bool IsInRange(ushort i)
    {
        if (i > _max) return false;
        if (i < _min) return false;
        return true;
    }

    public ushort PutInRange(ushort f, bool throwException = true)
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

    public bool IsInRange(RangeUInt16 r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeUInt16 PutInRange(RangeUInt16 r, bool throwException = true)
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
            ushort min = r._min < _min ? _min : r._min;
            ushort max = r._max < _max ? _max : r._max;
            return new RangeUInt16(min, max);
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not RangeUInt16 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeUInt16 other)
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

    public static bool operator ==(RangeUInt16 c1, RangeUInt16 c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeUInt16 c1, RangeUInt16 c2)
    {
        return !c1.Equals(c2);
    }

    public IEnumerator<ushort> GetEnumerator()
    {
        for (ushort i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}