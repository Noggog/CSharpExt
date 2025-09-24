using System.Globalization;
using System.Runtime.Serialization;

namespace Noggog;

public struct P2Double : IEquatable<P2Double>
{
    private double _x;
    [DataMember]
    public double X
    {
        get => _x;
        set => _x = value;
    }
        
    private double _y;
    [DataMember]
    public double Y
    {
        get => _y;
        set => _y = value;
    }

    [IgnoreDataMember]
    public double Length => Math.Sqrt(_x * _x + _y * _y);
    [IgnoreDataMember]
    public double Magnitude => Length;
    [IgnoreDataMember]
    public double SqrMagnitude => (_x * _x + _y * _y);

    [IgnoreDataMember]
    public P2Double Normalized
    {
        get
        {
            double length = Length;
            return new P2Double(_x / length, _y / length);
        }
    }

    [IgnoreDataMember]
    public P2Double Absolute => new P2Double(
        Math.Abs(_x),
        Math.Abs(_y));

    public P2Double(double x, double y)
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

    public P2Double Normalize()
    {
        var length = Length;
        return new P2Double(
            _x / length,
            _y / length);
    }

    public static double Dot(P2Double v1, P2Double v2) => v1._x * v2._x + v1._y * v2._y;

    public double Distance(P2Double p2) => (this - p2).Magnitude;

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P2Double p2, IFormatProvider? provider = null)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 2)
        {
            p2 = default(P2Double);
            return false;
        }

        if (!double.TryParse(split[0], NumberStyles.Any, provider, out double x))
        {
            p2 = default(P2Double);
            return false;
        }
        if (!double.TryParse(split[1], NumberStyles.Any, provider, out double y))
        {
            p2 = default(P2Double);
            return false;
        }
        p2 = new P2Double(x, y);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P2Double p2, IFormatProvider? provider = null)
    {
        double? x2 = null;
        double? y2 = null;

        var index = 0;
        foreach (var subStrSpan in str.Split(','))
        {
            switch (index)
            {
                case 0:
                {
                    if (!double.TryParse(subStrSpan, NumberStyles.Any, provider, out var x))
                    {
                        p2 = default;
                        return false;
                    }

                    x2 = x;
                    break;
                }
                case 1:
                {
                    if (!double.TryParse(subStrSpan, NumberStyles.Any, provider, out var y))
                    {
                        p2 = default;
                        return false;
                    }

                    y2 = y;
                    break;
                }
                default:
                    p2 = default;
                    return false;
            }

            index++;
        }

        if (x2 == null || y2 == null)
        {
            p2 = default;
            return false;
        }

        p2 = new P2Double(x2.Value, y2.Value);
        return true;
    }
#endif

    public override bool Equals(object? obj)
    {
        if (obj is not P2Double rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P2Double rhs)
    {
        return _x.EqualsWithin(rhs._x)
               && _y.EqualsWithin(rhs._y);
    }

    public override int GetHashCode() => HashCode.Combine(_x, _y);

    public static P2Double Max(double p1, double p2, double c1, double c2)
    {
        return new P2Double(Math.Max(p1, c1), Math.Max(p2, c2));
    }

    public static P2Double Max(P2Double p, P2Double c)
    {
        return new P2Double(Math.Max(p._x, c._x), Math.Max(p._y, c._y));
    }

    public P2Double Max(double c)
    {
        return new P2Double(Math.Max(_x, c), Math.Max(_y, c));
    }

    public static bool operator ==(P2Double obj1, P2Double obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P2Double obj1, P2Double obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P2Double operator -(P2Double c1)
    {
        return new P2Double(-c1._x, -c1._y);
    }

    public static P2Double operator +(P2Double c1, P2Double c2)
    {
        return new P2Double(c1._x + c2._x, c1._y + c2._y);
    }

    public static P2Double operator +(P2Double c1, double f)
    {
        return new P2Double(c1._x + f, c1._y + f);
    }

    public static P2Double operator -(P2Double c1, P2Double c2)
    {
        return new P2Double(c1._x - c2._x, c1._y - c2._y);
    }

    public static P2Double operator -(P2Double c1, double f)
    {
        return new P2Double(c1._x - f, c1._y - f);
    }

    public static P2Double operator *(P2Double c1, P2Double c2)
    {
        return new P2Double(c1._x * c2._x, c1._y * c2._y);
    }

    public static P2Double operator *(P2Double c1, double f)
    {
        return new P2Double(c1._x * f, c1._y * f);
    }

    public static P2Double operator /(P2Double c1, P2Double c2)
    {
        return new P2Double(c1._x / c2._x, c1._y / c2._y);
    }

    public static P2Double operator /(P2Double c1, double f)
    {
        return new P2Double(c1._x / f, c1._y / f);
    }

    public static implicit operator P2Double(P2Int point)
    {
        return new P2Double(point.X, point.Y);
    }

    public static IEqualityComparer<P2Double?> NullableRawEqualityComparer => new NullableRawEqualityComparerImpl();

    private class NullableRawEqualityComparerImpl : IEqualityComparer<P2Double?>
    {
        public bool Equals(P2Double? x, P2Double? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Value.X == y.Value.X && x.Value.Y == y.Value.Y;
        }

        public int GetHashCode(P2Double? obj)
        {
            if (obj == null) return 0;
            HashCode ret = new();
            ret.Add(obj.Value.X);
            ret.Add(obj.Value.Y);
            return ret.GetHashCode();
        }
    }
}