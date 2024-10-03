using System.Runtime.Serialization;

namespace Noggog;

public interface IP3IntGet
{
    [DataMember]
    int X { get; }
    [DataMember]
    int Y { get; }
    [DataMember]
    int Z { get; }
    [IgnoreDataMember]
    P3Int Point { get; }
}

public struct P3Int : IP3IntGet, IEquatable<P3Int>
{
    public static readonly P3Int Origin = new(0, 0, 0);
    public static readonly P3Int One = new(1, 1, 1);

    private int _x;
    [DataMember]
    public int X
    {
        get => _x;
        set => _x = value;
    }
        
    private int _y;
    [DataMember]
    public int Y
    {
        get => _y;
        set => _y = value;
    }
        
    private int _z;
    [DataMember]
    public int Z
    {
        get => _z;
        set => _z = value;
    }

    [IgnoreDataMember]
    public P3Int Point => this;

    public P3Int(int x, int y, int z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

#if NETSTANDARD2_0
    public static bool TryParse(string str, out P3Int ret)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.Split(',');
        if (split.Length != 3)
        {
            ret = default(P3Int);
            return false;
        }

        if (!int.TryParse(split[0], out int x)
            || !int.TryParse(split[1], out int y)
            || !int.TryParse(split[2], out int z))
        {
            ret = default(P3Int);
            return false;
        }

        ret = new P3Int(x, y, z);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P3Int ret)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.ToString().Split(',');
        if (split.Length != 3)
        {
            ret = default(P3Int);
            return false;
        }

        if (!int.TryParse(split[0], out int x)
            || !int.TryParse(split[1], out int y)
            || !int.TryParse(split[2], out int z))
        {
            ret = default(P3Int);
            return false;
        }

        ret = new P3Int(x, y, z);
        return true;
    }
#endif

    public P3Int Shift(int x, int y, int z)
    {
        return new P3Int(_x + x, _y + y, _z + z);
    }

    public P3Int Shift(P3Int p)
    {
        return Shift(p._x, p._y, p._z);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P3Int rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3Int rhs)
    {
        return _x == rhs._x
               && _y == rhs._y
               && _z == rhs._z;
    }

    public override int GetHashCode() => HashCode.Combine(_x, _y, _z);

    public override string ToString()
    {
        return $"{_x},{_y},{_z}";
    }

    public static bool operator ==(P3Int obj1, P3Int obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P3Int obj1, P3Int obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P3Int operator +(P3Int p1, P3Int p2)
    {
        return p1.Shift(p2);
    }

    public static P3Int operator +(P3Int p1, int p)
    {
        return new P3Int(p1._x + p, p1._y + p, p1._z + p);
    }

    public static P3Int operator -(P3Int p1, P3Int p2)
    {
        return new P3Int(p1._x - p2._x, p1._y - p2._y, p1._z - p2._z);
    }

    public static P3Int operator -(P3Int p1, int p)
    {
        return new P3Int(p1._x - p, p1._y - p, p1._z - p);
    }

    public static P3Int operator -(P3Int p1)
    {
        return new P3Int(-p1._x, -p1._y, -p1._z);
    }

    public static P3Int operator *(P3Int p1, int num)
    {
        return new P3Int(p1._x * num, p1._y * num, p1._z * num);
    }

    public static P3Int operator *(P3Int p1, P3Int p2)
    {
        return new P3Int(p1._x * p2._x, p1._y * p2._y, p1._z * p2._z);
    }

    public static P3Int operator /(P3Int p1, int num)
    {
        return new P3Int(p1._x / num, p1._y / num, p1._z / num);
    }

    public static explicit operator P3Double(P3Int point)
    {
        return new P3Double(point._x, point._y, point._z);
    }
}