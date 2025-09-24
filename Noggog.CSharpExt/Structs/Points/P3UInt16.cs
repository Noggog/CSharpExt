using System.Globalization;
using System.Runtime.Serialization;

namespace Noggog;

public interface IP3UInt16Get
{
    [DataMember]
    ushort X { get; }
    [DataMember]
    ushort Y { get; }
    [DataMember]
    ushort Z { get; }
    [IgnoreDataMember]
    P3UInt16 Point { get; }
}

public struct P3UInt16 : IP3UInt16Get, IEquatable<P3UInt16>
{
    public static readonly P3UInt16 Origin = new(0, 0, 0);
    public static readonly P3UInt16 One = new(1, 1, 1);

    private ushort _x;
    [DataMember]
    public ushort X
    {
        get => _x;
        set => _x = value;
    }
        
    private ushort _y;
    [DataMember]
    public ushort Y
    {
        get => _y;
        set => _y = value;
    }
        
    private ushort _z;
    [DataMember]
    public ushort Z
    {
        get => _z;
        set => _z = value;
    }

    [IgnoreDataMember]
    public P3UInt16 Point => this;

    public P3UInt16(ushort x, ushort y, ushort z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P3UInt16 ret, IFormatProvider? provider = null)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 3)
        {
            ret = default(P3UInt16);
            return false;
        }

        if (!ushort.TryParse(split[0], NumberStyles.Any, provider, out ushort x)
            || !ushort.TryParse(split[1], NumberStyles.Any, provider, out ushort y)
            || !ushort.TryParse(split[2], NumberStyles.Any, provider, out ushort z))
        {
            ret = default(P3UInt16);
            return false;
        }

        ret = new P3UInt16(x, y, z);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P3UInt16 ret, IFormatProvider? provider = null)
    {
        ushort? x2 = null;
        ushort? y2 = null;
        ushort? z2 = null;

        var index = 0;
        foreach (var subStrSpan in str.Split(','))
        {
            switch (index)
            {
                case 0:
                {
                    if (!ushort.TryParse(subStrSpan, NumberStyles.Any, provider, out var x))
                    {
                        ret = default;
                        return false;
                    }

                    x2 = x;
                    break;
                }
                case 1:
                {
                    if (!ushort.TryParse(subStrSpan, NumberStyles.Any, provider, out var y))
                    {
                        ret = default;
                        return false;
                    }

                    y2 = y;
                    break;
                }
                case 2:
                {
                    if (!ushort.TryParse(subStrSpan, NumberStyles.Any, provider, out var z))
                    {
                        ret = default;
                        return false;
                    }

                    z2 = z;
                    break;
                }
                default:
                    ret = default;
                    return false;
            }

            index++;
        }

        if (x2 == null || y2 == null || z2 == null)
        {
            ret = default;
            return false;
        }

        ret = new P3UInt16(x2.Value, y2.Value, z2.Value);
        return true;
    }
#endif

    public P3UInt16 Shift(ushort x, ushort y, ushort z)
    {
        return new P3UInt16((ushort)(_x + x), (ushort)(_y + y), (ushort)(_z + z));
    }

    public P3UInt16 Shift(P3UInt16 p)
    {
        return Shift(p._x, p._y, p._z);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P3UInt16 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3UInt16 rhs)
    {
        return _x == rhs._x
               && _y == rhs._y
               && _z == rhs._z;
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

    public static bool operator ==(P3UInt16 obj1, P3UInt16 obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P3UInt16 obj1, P3UInt16 obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P3UInt16 operator +(P3UInt16 p1, P3UInt16 p2)
    {
        return p1.Shift(p2);
    }

    public static P3UInt16 operator +(P3UInt16 p1, int p)
    {
        return new P3UInt16((ushort)(p1._x + p), (ushort)(p1._y + p), (ushort)(p1._z + p));
    }

    public static P3UInt16 operator -(P3UInt16 p1, P3UInt16 p2)
    {
        return new P3UInt16((ushort)(p1._x - p2._x), (ushort)(p1._y - p2._y), (ushort)(p1._z - p2._z));
    }

    public static P3UInt16 operator -(P3UInt16 p1, ushort p)
    {
        return new P3UInt16((ushort)(p1._x - p), (ushort)(p1._y - p), (ushort)(p1._z - p));
    }

    public static P3UInt16 operator -(P3UInt16 p1)
    {
        return new P3UInt16((ushort)(-p1._x), (ushort)(-p1._y), (ushort)(-p1._z));
    }

    public static P3UInt16 operator *(P3UInt16 p1, ushort num)
    {
        return new P3UInt16((ushort)(p1._x * num), (ushort)(p1._y * num), (ushort)(p1._z * num));
    }

    public static P3UInt16 operator *(P3UInt16 p1, P3UInt16 p2)
    {
        return new P3UInt16((ushort)(p1._x * p2._x), (ushort)(p1._y * p2._y), (ushort)(p1._z * p2._z));
    }

    public static P3UInt16 operator /(P3UInt16 p1, ushort num)
    {
        return new P3UInt16((ushort)(p1._x / num), (ushort)(p1._y / num), (ushort)(p1._z / num));
    }

    public static explicit operator P3Double(P3UInt16 point)
    {
        return new P3Double(point._x, point._y, point._z);
    }
}