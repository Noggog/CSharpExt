using System.Runtime.Serialization;

namespace Noggog;

public interface IP2Int16Get
{
    [DataMember]
    short X { get; }
    [DataMember]
    short Y { get; }
    [IgnoreDataMember]
    P2Int16 Point { get; }
}

public struct P2Int16 : IP2Int16Get, IEquatable<P2Int16>
{
    public static readonly P2Int16 Origin = new(0, 0);
    public static readonly P2Int16 One = new(1, 1);
    [IgnoreDataMember]
    public bool IsZero => X == 0 && _y == 0;
        
    [IgnoreDataMember] 
    P2Int16 IP2Int16Get.Point => this; 

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

    #region Ctors
    public P2Int16(short x, short y)
    {
        _x = x;
        _y = y;
    }

    public P2Int16(IP2Int16Get rhs)
        : this(rhs.X, rhs.Y)
    {
    }
    #endregion Ctors

    #region Shifts
    public P2Int16 Shift(short x, short y)
    {
        return new P2Int16((short)(X + x), (short)(_y + y));
    }

    public P2Int16 Shift(P2Int16 p)
    {
        return Shift(p.X, p.Y);
    }
    #endregion Shifts

    public double Distance(P2Int16 rhs)
    {
        return Distance(rhs.X, rhs._y);
    }

    public double Distance(short x, short y)
    {
        return Math.Sqrt(Math.Pow(x - X, 2) + Math.Pow(y - _y, 2));
    }

    public P2Int16 Invert()
    {
        return new P2Int16((short)-X, (short)-_y);
    }
        
    public override string ToString()
    {
        return $"({X},{_y})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P2Int16 p) return false;
        return Equals(p);
    }

    public bool Equals(P2Int16 rhs)
    {
        return X == rhs.X
               && _y == rhs._y;
    }

    public static bool TryParse(string str, out P2Int16 ret)
    {
        string[] split = str.Split(',');
        if (split.Length != 2)
        {
            ret = default(P2Int16);
            return false;
        }

        if (!short.TryParse(split[0], out var x)
            || !short.TryParse(split[1], out var y))
        {
            ret = default(P2Int16);
            return false;
        }

        ret = new P2Int16(x, y);
        return true;
    }

    public override int GetHashCode() => HashCode.Combine(X, _y);

    public static bool operator ==(P2Int16 obj1, P2Int16 obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P2Int16 obj1, P2Int16 obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P2Int16 operator +(P2Int16 p1, P2Int16 p2)
    {
        return p1.Shift(p2);
    }

    public static P2Int16 operator -(P2Int16 p1, P2Int16 p2)
    {
        return new P2Int16((short)(p1.X - p2.X), (short)(p1._y - p2._y));
    }

    public static P2Int16 operator -(P2Int16 p1)
    {
        return new P2Int16((short)-p1.X, (short)-p1._y);
    }

    public static P2Int16 operator *(P2Int16 p1, short num)
    {
        return new P2Int16((short)(p1.X * num), (short)(p1._y * num));
    }
}