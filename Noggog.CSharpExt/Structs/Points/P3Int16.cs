using System.Runtime.Serialization;

namespace Noggog;

public interface IP3Int16Get
{
    [DataMember]
    short X { get; }
    [DataMember]
    short Y { get; }
    [DataMember]
    short Z { get; }
    [IgnoreDataMember]
    P3Int16 Point { get; }
}

public struct P3Int16 : IP3Int16Get, IEquatable<P3Int16>
{
    public static readonly P3Int16 Origin = new(0, 0, 0);
    public static readonly P3Int16 One = new(1, 1, 1);

    private short _x;
    [DataMember]
    public short X
    {
        get => _x;
        set => _x = value;
    }
        
    private short _y;
    [DataMember]
    public short Y
    {
        get => _y;
        set => _y = value;
    }
        
    private short _z;
    [DataMember]
    public short Z
    {
        get => _z;
        set => _z = value;
    }

    [IgnoreDataMember]
    public P3Int16 Point => this;

    public P3Int16(short x, short y, short z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P3Int16 ret)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 3)
        {
            ret = default(P3Int16);
            return false;
        }

        if (!short.TryParse(split[0], out short x)
            || !short.TryParse(split[1], out short y)
            || !short.TryParse(split[2], out short z))
        {
            ret = default(P3Int16);
            return false;
        }

        ret = new P3Int16(x, y, z);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P3Int16 ret)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.ToString().Split(',');
        if (split.Length != 3)
        {
            ret = default(P3Int16);
            return false;
        }

        if (!short.TryParse(split[0], out short x)
            || !short.TryParse(split[1], out short y)
            || !short.TryParse(split[2], out short z))
        {
            ret = default(P3Int16);
            return false;
        }

        ret = new P3Int16(x, y, z);
        return true;
    }
#endif

    public P3Int16 Shift(short x, short y, short z)
    {
        return new P3Int16((short)(_x + x), (short)(_y + y), (short)(_z + z));
    }

    public P3Int16 Shift(P3Int16 p)
    {
        return Shift(p._x, p._y, p._z);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P3Int16 rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3Int16 rhs)
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

    public static bool operator ==(P3Int16 obj1, P3Int16 obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P3Int16 obj1, P3Int16 obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P3Int16 operator +(P3Int16 p1, P3Int16 p2)
    {
        return p1.Shift(p2);
    }

    public static P3Int16 operator +(P3Int16 p1, int p)
    {
        return new P3Int16((short)(p1._x + p), (short)(p1._y + p), (short)(p1._z + p));
    }

    public static P3Int16 operator -(P3Int16 p1, P3Int16 p2)
    {
        return new P3Int16((short)(p1._x - p2._x), (short)(p1._y - p2._y), (short)(p1._z - p2._z));
    }

    public static P3Int16 operator -(P3Int16 p1, short p)
    {
        return new P3Int16((short)(p1._x - p), (short)(p1._y - p), (short)(p1._z - p));
    }

    public static P3Int16 operator -(P3Int16 p1)
    {
        return new P3Int16((short)(-p1._x), (short)(-p1._y), (short)(-p1._z));
    }

    public static P3Int16 operator *(P3Int16 p1, short num)
    {
        return new P3Int16((short)(p1._x * num), (short)(p1._y * num), (short)(p1._z * num));
    }

    public static P3Int16 operator *(P3Int16 p1, P3Int16 p2)
    {
        return new P3Int16((short)(p1._x * p2._x), (short)(p1._y * p2._y), (short)(p1._z * p2._z));
    }

    public static P3Int16 operator /(P3Int16 p1, short num)
    {
        return new P3Int16((short)(p1._x / num), (short)(p1._y / num), (short)(p1._z / num));
    }

    public static explicit operator P3Double(P3Int16 point)
    {
        return new P3Double(point._x, point._y, point._z);
    }
}