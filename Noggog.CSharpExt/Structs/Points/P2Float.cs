using System.Globalization;
using System.Runtime.Serialization;

namespace Noggog;

public struct P2Float : IEquatable<P2Float>
{
    private float _x;
    [DataMember]
    public float X
    {
        get => _x;
        set => _x = value;
    }
        
    private float _y;
    [DataMember]
    public float Y
    {
        get => _y;
        set => _y = value;
    }

    [IgnoreDataMember]
    public float Length => (float)Math.Sqrt(_x * _x + _y * _y);
    [IgnoreDataMember]
    public float Magnitude => Length;
    [IgnoreDataMember]
    public float SqrMagnitude => (_x * _x + _y * _y);

    [IgnoreDataMember]
    public P2Float Normalized
    {
        get
        {
            float length = Length;
            return new P2Float(_x / length, _y / length);
        }
    }

    [IgnoreDataMember]
    public P2Float Absolute => new P2Float(
        Math.Abs(_x),
        Math.Abs(_y));

    public P2Float(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public override string ToString()
    {
        return $"{_x}, {_y}";
    }

    public string ToString(IFormatProvider? provider)
    {
        return $"{_x.ToString(provider)}, {_y.ToString(provider)}";
    }

    public P2Float Normalize()
    {
        var length = Length;
        return new P2Float(
            _x / length,
            _y / length);
    }

    public float Cross(P2Float b)
    {
        return this.X * b.Y - this.Y * b.X;
    }

    public float Dot(P2Float b)
    {
        return this.X * b.X + this.Y * b.Y;
    }

    public static float Dot(P2Float v1, P2Float v2) => v1._x * v2._x + v1._y * v2._y;
    public float Distance(P2Float p2) => (this - p2).Magnitude;

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P2Float p2, IFormatProvider? provider = null)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 2)
        {
            p2 = default(P2Float);
            return false;
        }

        if (!float.TryParse(split[0], NumberStyles.Any, provider, out float x))
        {
            p2 = default(P2Float);
            return false;
        }
        if (!float.TryParse(split[1], NumberStyles.Any, provider, out float y))
        {
            p2 = default(P2Float);
            return false;
        }
        p2 = new P2Float(x, y);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P2Float p2, IFormatProvider? provider = null)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.ToString().Split(',');
        if (split.Length != 2)
        {
            p2 = default(P2Float);
            return false;
        }
        
        if (!float.TryParse(split[0], NumberStyles.Any, provider, out float x))
        {
            p2 = default(P2Float);
            return false;
        }
        if (!float.TryParse(split[1], NumberStyles.Any, provider, out float y))
        {
            p2 = default(P2Float);
            return false;
        }
        p2 = new P2Float(x, y);
        return true;
    }
#endif

    public override bool Equals(object? obj)
    {
        if (obj is not P2Float rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P2Float rhs)
    {
        return _x.EqualsWithin(rhs._x)
               && _y.EqualsWithin(rhs._y);
    }

    public override int GetHashCode() => HashCode.Combine(_x, _y);

    public static P2Float Max(P2Float p, P2Float c)
    {
        return new P2Float(Math.Max(p._x, c._x), Math.Max(p._y, c._y));
    }

    public P2Float Max(float c)
    {
        return new P2Float(Math.Max(_x, c), Math.Max(_y, c));
    }

    public static bool operator ==(P2Float obj1, P2Float obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P2Float obj1, P2Float obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P2Float operator -(P2Float c1)
    {
        return new P2Float(-c1._x, -c1._y);
    }

    public static P2Float operator +(P2Float c1, P2Float c2)
    {
        return new P2Float(c1._x + c2._x, c1._y + c2._y);
    }

    public static P2Float operator +(P2Float c1, float f)
    {
        return new P2Float(c1._x + f, c1._y + f);
    }

    public static P2Float operator -(P2Float c1, P2Float c2)
    {
        return new P2Float(c1._x - c2._x, c1._y - c2._y);
    }

    public static P2Float operator -(P2Float c1, float f)
    {
        return new P2Float(c1._x - f, c1._y - f);
    }

    public static P2Float operator *(P2Float c1, P2Float c2)
    {
        return new P2Float(c1._x * c2._x, c1._y * c2._y);
    }

    public static P2Float operator *(P2Float c1, float f)
    {
        return new P2Float(c1._x * f, c1._y * f);
    }

    public static P2Float operator /(P2Float c1, P2Float c2)
    {
        return new P2Float(c1._x / c2._x, c1._y / c2._y);
    }

    public static P2Float operator /(P2Float c1, float f)
    {
        return new P2Float(c1._x / f, c1._y / f);
    }

    public static implicit operator P2Float(P2Int point)
    {
        return new P2Float(point.X, point.Y);
    }

    public static IEqualityComparer<P2Float?> NullableRawEqualityComparer => new NullableRawEqualityComparerImpl();

    private class NullableRawEqualityComparerImpl : IEqualityComparer<P2Float?>
    {
        public bool Equals(P2Float? x, P2Float? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Value.X == y.Value.X && x.Value.Y == y.Value.Y;
        }

        public int GetHashCode(P2Float? obj)
        {
            if (obj == null) return 0;
            HashCode ret = new();
            ret.Add(obj.Value.X);
            ret.Add(obj.Value.Y);
            return ret.GetHashCode();
        }
    }
}