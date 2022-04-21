using System.Collections;

namespace Noggog;

public struct RangeUInt8 : IEquatable<RangeUInt8>, IEnumerable<byte>
{
    private byte _min;
    public byte Min
    {
        get => _min;
        set => _min = value;
    }

    private byte _max;
    public byte Max
    {
        get => _max;
        set => _max = value;
    }
        
    public float Average => ((_max - _min) / 2f) + _min;
    public byte Difference => (byte)(_max - _min);
    public ushort Width => (byte)(_max - _min + 1);

    public RangeUInt8(byte val1, byte val2)
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

    public RangeUInt8(byte? min, byte? max)
        : this(min ?? byte.MinValue, max ?? byte.MaxValue)
    {
    }

    public RangeUInt8(byte val)
        : this(val, val)
    {
    }

    public static RangeUInt8 FromLength(byte loc, byte length)
    {
        return new RangeUInt8(
            loc,
            (byte)(loc + length - 1));
    }

    public static RangeUInt8 Parse(string str)
    {
        if (!TryParse(str, out RangeUInt8 rd))
        {
            return default(RangeUInt8);
        }
        return rd;
    }

    public static bool TryParse(string str, out RangeUInt8 rd)
    {
        string[] split = str.Split('-');
        if (split.Length != 2)
        {
            rd = default(RangeUInt8);
            return false;
        }
        rd = new RangeUInt8(
            byte.Parse(split[0]),
            byte.Parse(split[1]));
        return true;
    }

    public bool IsInRange(byte i)
    {
        if (i > _max) return false;
        if (i < _min) return false;
        return true;
    }

    public byte PutInRange(byte f, bool throwException = true)
    {
        if (throwException)
        {
            if (f < _min)
            {
                throw new ArgumentException($"Nin is out of range: {f} < {_min}");
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

    public bool IsInRange(RangeUInt8 r)
    {
        if (r._max > _max) return false;
        if (r._min < _min) return false;
        return true;
    }

    public RangeUInt8 PutInRange(RangeUInt8 r, bool throwException = true)
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
            byte min = r._min < _min ? _min : r._min;
            byte max = r._max < _max ? _max : r._max;
            return new RangeUInt8(min, max);
        }
    }
        
    public override bool Equals(object? obj)
    {
        if (obj is not RangeUInt8 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(RangeUInt8 other)
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

    public static bool operator ==(RangeUInt8 c1, RangeUInt8 c2)
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(RangeUInt8 c1, RangeUInt8 c2)
    {
        return !c1.Equals(c2);
    }

    public IEnumerator<byte> GetEnumerator()
    {
        for (byte i = _min; i <= _max; i++)
        {
            yield return i;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}