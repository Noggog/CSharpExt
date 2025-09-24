using System.Globalization;
using System.Runtime.Serialization;

namespace Noggog;

public struct P3Double : IEquatable<P3Double>
{
    public static readonly P3Double Origin = Zero;
    public static readonly P3Double Zero = new(0, 0, 0);
    public static readonly P3Double One = new(1, 1, 1);
    public static readonly P3Double Up = new(0, 1, 0);
    public static readonly P3Double Down = new(0, -1, 0);
    public static readonly P3Double Back = new(0, 0, -1);
    public static readonly P3Double Forward = new(0, 0, 1);
    public static readonly P3Double Left = new(-1, 0, 0);
    public static readonly P3Double Right = new(1, 0, 0);

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

    private double _z;

    [DataMember]
    public double Z
    {
        get => _z;
        set => _z = value;
    }

    [IgnoreDataMember] public P3Double XPoint => new(_x, 0, 0);
    [IgnoreDataMember] public P3Double YPoint => new(0, _y, 0);
    [IgnoreDataMember] public P3Double ZPoint => new(0, 0, _z);

    [IgnoreDataMember] public P2Double XY => new(_x, _y);
    [IgnoreDataMember] public P2Double XZ => new(_x, _z);
    [IgnoreDataMember] public P2Double YZ => new(_y, _z);
    [IgnoreDataMember] public P2Double ZY => new(_z, _y);
    [IgnoreDataMember] public P2Double ZX => new(_z, _x);
    [IgnoreDataMember] public P2Double YX => new(_y, _x);

    [IgnoreDataMember] public P2Double XX => new(_x, _x);
    [IgnoreDataMember] public P2Double YY => new(_y, _y);
    [IgnoreDataMember] public P2Double ZZ => new(_z, _z);

    [IgnoreDataMember] public P3Double XXX => new(_x, _x, _x);
    [IgnoreDataMember] public P3Double XXY => new(_x, _x, _y);
    [IgnoreDataMember] public P3Double XXZ => new(_x, _x, _z);
    [IgnoreDataMember] public P3Double XYX => new(_x, _y, _x);
    [IgnoreDataMember] public P3Double XYY => new(_x, _y, _y);
    [IgnoreDataMember] public P3Double XYZ => new(_x, _y, _z);
    [IgnoreDataMember] public P3Double XZX => new(_x, _z, _x);
    [IgnoreDataMember] public P3Double XZY => new(_x, _z, _y);
    [IgnoreDataMember] public P3Double XZZ => new(_x, _z, _z);

    [IgnoreDataMember] public P3Double YXX => new(_y, _x, _x);
    [IgnoreDataMember] public P3Double YXY => new(_y, _x, _y);
    [IgnoreDataMember] public P3Double YXZ => new(_y, _x, _z);
    [IgnoreDataMember] public P3Double YYX => new(_y, _y, _x);
    [IgnoreDataMember] public P3Double YYY => new(_y, _y, _y);
    [IgnoreDataMember] public P3Double YYZ => new(_y, _y, _z);
    [IgnoreDataMember] public P3Double YZX => new(_y, _z, _x);
    [IgnoreDataMember] public P3Double YZY => new(_y, _z, _y);
    [IgnoreDataMember] public P3Double YZZ => new(_y, _z, _z);

    [IgnoreDataMember] public P3Double ZXX => new(_z, _x, _x);
    [IgnoreDataMember] public P3Double ZXY => new(_z, _x, _y);
    [IgnoreDataMember] public P3Double ZXZ => new(_z, _x, _z);
    [IgnoreDataMember] public P3Double ZYX => new(_z, _y, _x);
    [IgnoreDataMember] public P3Double ZYY => new(_z, _y, _y);
    [IgnoreDataMember] public P3Double ZYZ => new(_z, _y, _z);
    [IgnoreDataMember] public P3Double ZZX => new(_z, _z, _x);
    [IgnoreDataMember] public P3Double ZZY => new(_z, _z, _y);
    [IgnoreDataMember] public P3Double ZZZ => new(_z, _z, _z);

    [IgnoreDataMember]
    public P3Double Normalized
    {
        get
        {
            double length = Length;
            return new P3Double(_x / length, _y / length, _z / length);
        }
    }

    [IgnoreDataMember] public double Length => Math.Sqrt(_x * _x + _y * _y + _z * _z);

    [IgnoreDataMember] public double Magnitude => Length;

    [IgnoreDataMember] public double SqrMagnitude => (_x * _x + _y * _y + _z * _z);

    [IgnoreDataMember]
    public P3Double Absolute => new(
        Math.Abs(_x),
        Math.Abs(_y),
        Math.Abs(_z));

    public P3Double(double x, double y, double z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public P3Double(P3Double p2)
    {
        _x = p2._x;
        _y = p2._y;
        _z = p2._z;
    }

    public P3Double Shift(double x, double y, double z)
    {
        return new P3Double(_x + x, _y + y, _z + z);
    }

    public P3Double Shift(float x, float y, float z)
    {
        return new P3Double(_x + x, _y + y, _z + z);
    }

    public P3Double Shift(P3Double p)
    {
        return Shift(p._x, p._y, p._z);
    }

    public double Distance(P3Double p2)
    {
        return (this - p2).Length;
    }

    public static double Distance(P3Double p1, P3Double p2)
    {
        return (p1 - p2).Length;
    }

    public P3Double Set_x(double x)
    {
        return new P3Double(x, _y, _z);
    }

    public P3Double Set_y(double y)
    {
        return new P3Double(_x, y, _z);
    }

    public P3Double Set_z(double z)
    {
        return new P3Double(_x, _y, z);
    }

    public P3Double Modify_x(double x)
    {
        return new P3Double(_x + x, _y, _z);
    }

    public P3Double Modify_y(double y)
    {
        return new P3Double(_x, _y + y, _z);
    }

    public P3Double Modify_z(double z)
    {
        return new P3Double(_x, _y, _z + z);
    }

    public bool EqualsWithin(double value, double within = .000000001d)
    {
        return _x.EqualsWithin(value, within) && _y.EqualsWithin(value, within) && _z.EqualsWithin(value, within);
    }

    public bool EqualsWithin(P3Double value, double within = .000000001d)
    {
        return _x.EqualsWithin(value._x, within) && _y.EqualsWithin(value._y, within) &&
               _z.EqualsWithin(value._z, within);
    }

    public static P3Double ProjectOnPlane(P3Double v, P3Double planeNormal)
    {
        var distance = -Dot(planeNormal.Normalized, v);
        return v + planeNormal * distance;
    }

    public static double Dot(P3Double v1, P3Double v2)
    {
        return v1._x * v2._x + v1._y * v2._y + v1._z * v2._z;
    }

    public P3Double Process(Func<double, double> conv)
    {
        return new P3Double(
            conv(_x),
            conv(_y),
            conv(_z));
    }

    public static P3Double Cross(P3Double v1, P3Double v2)
    {
        double x, y, z;
        x = v1._y * v2._z - v2._y * v1._z;
        y = (v1._x * v2._z - v2._x * v1._z) * -1;
        z = v1._x * v2._y - v2._x * v1._y;

        var rtnvector = new P3Double(x, y, z);
        return rtnvector;
    }

    public static P3Double Lerp(P3Double start, P3Double end, double percent)
    {
        percent.Clamp01();
        return start + percent * (end - start);
    }

    public P3Double Max(P3Double p2)
    {
        return new P3Double(Math.Max(_x, p2._x), Math.Max(_y, p2._y), Math.Max(_z, p2._z));
    }

    public static P3Double Max(P3Double p1, P3Double p2)
    {
        return new P3Double(Math.Max(p1._x, p2._x), Math.Max(p1._y, p2._y), Math.Max(p1._z, p2._z));
    }

    public P3Double Min(P3Double p2)
    {
        return new P3Double(Math.Min(_x, p2._x), Math.Min(_y, p2._y), Math.Min(_z, p2._z));
    }

    public static P3Double Min(P3Double p1, P3Double p2)
    {
        return new P3Double(Math.Min(p1._x, p2._x), Math.Min(p1._y, p2._y), Math.Min(p1._z, p2._z));
    }

    public static P3Double Abs(P3Double p1)
    {
        return p1.Absolute;
    }

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P3Double p3, IFormatProvider? provider = null)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 3)
        {
            p3 = default(P3Double);
            return false;
        }

        if (!double.TryParse(split[0], NumberStyles.Any, provider, out double x))
        {
            p3 = default(P3Double);
            return false;
        }

        if (!double.TryParse(split[1], NumberStyles.Any, provider, out double y))
        {
            p3 = default(P3Double);
            return false;
        }

        if (!double.TryParse(split[2], NumberStyles.Any, provider, out double z))
        {
            p3 = default(P3Double);
            return false;
        }

        p3 = new P3Double(x, y, z);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P3Double p3, IFormatProvider? provider = null)
    {
        double? x2 = null;
        double? y2 = null;
        double? z2 = null;

        var index = 0;
        foreach (var subStrSpan in str.Split(','))
        {
            switch (index)
            {
                case 0:
                {
                    if (!double.TryParse(subStrSpan, NumberStyles.Any, provider, out var x))
                    {
                        p3 = default;
                        return false;
                    }

                    x2 = x;
                    break;
                }
                case 1:
                {
                    if (!double.TryParse(subStrSpan, NumberStyles.Any, provider, out var y))
                    {
                        p3 = default;
                        return false;
                    }

                    y2 = y;
                    break;
                }
                case 2:
                {
                    if (!double.TryParse(subStrSpan, NumberStyles.Any, provider, out var z))
                    {
                        p3 = default;
                        return false;
                    }

                    z2 = z;
                    break;
                }
                default:
                    p3 = default;
                    return false;
            }

            index++;
        }

        if (x2 == null || y2 == null || z2 == null)
        {
            p3 = default;
            return false;
        }

        p3 = new P3Double(x2.Value, y2.Value, z2.Value);
        return true;
    }
#endif

    public override bool Equals(object? obj)
    {
        if (obj is not P3Double rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3Double rhs)
    {
        return _x.EqualsWithin(rhs._x)
               && _y.EqualsWithin(rhs._y)
               && _z.EqualsWithin(rhs._z);
    }

    public override int GetHashCode() => HashCode.Combine(_x, _y, _z);

    public override string ToString()
    {
        return $"{_x}, {_y}, {_z}";
    }

    public string ToString(IFormatProvider? provider)
    {
        return $"{_x.ToString(provider)}, {_y.ToString(provider)}, {_z.ToString(provider)}";
    }

    public static bool operator ==(P3Double obj1, P3Double obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P3Double obj1, P3Double obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P3Double operator +(P3Double p1, P3Double p2)
    {
        return p1.Shift(p2);
    }

    public static P3Double operator +(P3Double p1, double num)
    {
        return new P3Double(p1._x + num, p1._y + num, p1._z + num);
    }

    public static P3Double operator -(P3Double p1, P3Double p2)
    {
        return new P3Double(p1._x - p2._x, p1._y - p2._y, p1._z - p2._z);
    }

    public static P3Double operator -(P3Double p1, double num)
    {
        return new P3Double(p1._x - num, p1._y - num, p1._z - num);
    }

    public static P3Double operator -(P3Double p1)
    {
        return new P3Double(-p1._x, -p1._y, -p1._z);
    }

    public static P3Double operator *(P3Double p1, int num)
    {
        return new P3Double(p1._x * num, p1._y * num, p1._z * num);
    }

    public static P3Double operator *(P3Double p1, double num)
    {
        return new P3Double(p1._x * num, p1._y * num, p1._z * num);
    }

    public static P3Double operator *(double num, P3Double p1)
    {
        return new P3Double(p1._x * num, p1._y * num, p1._z * num);
    }

    public static P3Double operator /(P3Double p1, double num)
    {
        return new P3Double(p1._x / num, p1._y / num, p1._z / num);
    }

    public static P3Double operator *(P3Double p1, P3Double p2)
    {
        return new P3Double(p1._x * p2._x, p1._y * p2._y, p1._z * p2._z);
    }

    public static explicit operator P2Double(P3Double point)
    {
        return new P2Double(point._x, point._y);
    }

    public static IEqualityComparer<P3Double?> NullableRawEqualityComparer => new NullableRawEqualityComparerImpl();

    private class NullableRawEqualityComparerImpl : IEqualityComparer<P3Double?>
    {
        public bool Equals(P3Double? x, P3Double? y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return x.Value.X == y.Value.X && x.Value.Y == y.Value.Y && x.Value.Z == y.Value.Z;
        }

        public int GetHashCode(P3Double? obj)
        {
            if (obj == null) return 0;
            HashCode ret = new();
            ret.Add(obj.Value.X);
            ret.Add(obj.Value.Y);
            ret.Add(obj.Value.Z);
            return ret.GetHashCode();
        }
    }
}
