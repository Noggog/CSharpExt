using System.Runtime.Serialization;

namespace Noggog;

public interface IP2IntGet
{
    [DataMember]
    int X { get; }
    [DataMember]
    int Y { get; }
    [IgnoreDataMember]
    P2Int Point { get; }
}

public class P2IntObj
{
    [IgnoreDataMember]
    public P2Int Point;
}

public struct P2Int : IP2IntGet, IEquatable<P2Int>
{
    private static readonly P2Int[] _directions = new[] { new P2Int(1, 0), new P2Int(-1, 0), new P2Int(0, 1), new P2Int(0, -1) };
    public static IEnumerable<P2Int> Directions => _directions;
    public static P2Int Down => _directions[3];
    public static P2Int Up => _directions[2];
    public static P2Int Left => _directions[0];
    public static P2Int Right => _directions[1];

    public static readonly P2Int Origin = new(0, 0);
    public static readonly P2Int One = new(1, 1);

    [IgnoreDataMember]
    public bool IsZero => _x == 0 && _y == 0;

    [IgnoreDataMember]
    P2Int IP2IntGet.Point => this;

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

    #region Ctors
    public P2Int(int x, int y)
    {
        _x = x;
        _y = y;
    }

    public P2Int(IP2IntGet rhs)
        : this(rhs.X, rhs.Y)
    {
    }
    #endregion Ctors

    #region Shifts
    public P2Int Shift(int x, int y)
    {
        return new P2Int(_x + x, _y + y);
    }

    public P2Int Shift(double x, double y)
    {
        return Shift((int)x, (int)y);
    }

    public P2Int Shift(P2Double vect)
    {
        return Shift(vect.X, vect.Y);
    }

    public P2Int Shift(P2Int p)
    {
        return Shift(p._x, p._y);
    }

    public P2Int ShiftToPositive()
    {
        return Shift(
            _x < 0 ? -_x : 0,
            _y < 0 ? -_y : 0);
    }
    #endregion Shifts

    public P2Int UnitDir()
    {
        int max = Math.Max(Math.Abs(_x), Math.Abs(_y));
        if (max != 0)
        {
            return new P2Int(
                (int)Math.Round(((decimal)_x) / max),
                (int)Math.Round(((decimal)_y) / max));
        }
        else
        {
            return new P2Int();
        }
    }

    public int MidPoint()
    {
        return (_y - _x) / 2;
    }

    public double Distance(P2Int rhs)
    {
        return Distance(rhs._x, rhs._y);
    }

    public double Distance(int x, int y)
    {
        return Math.Sqrt(Math.Pow(x - _x, 2) + Math.Pow(y - _y, 2));
    }

    public P2Int Invert()
    {
        return new P2Int(-_x, -_y);
    }
        
    public override string ToString()
    {
        return $"{_x}, {_y}";
    }

    public string ToString(IFormatProvider? provider)
    {
        return $"{_x.ToString(provider)}, {_y.ToString(provider)}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P2Int p) return false;
        return Equals(p);
    }

    public bool Equals(P2Int rhs)
    {
        return _x == rhs._x
               && _y == rhs._y;
    }
    
#if NETSTANDARD2_0
    public static bool TryParse(string str, out P2Int ret)
    {
        string[] split = str.Split(',');
        if (split.Length != 2)
        {
            ret = default(P2Int);
            return false;
        }

        if (!int.TryParse(split[0], out int x)
            || !int.TryParse(split[1], out int y))
        {
            ret = default(P2Int);
            return false;
        }

        ret = new P2Int(x, y);
        return true;
    }
#else 
    public static bool TryParse(ReadOnlySpan<char> str, out P2Int ret)
    {
        // ToDo
        // Improve parsing to reduce allocation
        string[] split = str.ToString().Split(',');
        if (split.Length != 2)
        {
            ret = default(P2Int);
            return false;
        }

        if (!int.TryParse(split[0], out int x)
            || !int.TryParse(split[1], out int y))
        {
            ret = default(P2Int);
            return false;
        }

        ret = new P2Int(x, y);
        return true;
    }
#endif

    public override int GetHashCode() => HashCode.Combine(_x, _y);

    public bool NextTo(P2Int p)
    {
        if (p._x == _x)
        {
            return p._y == _y + 1 || p._y == _y - 1;
        }
        if (p._y == _y)
        {
            return p._x == _x + 1 || p._x == _x - 1;
        }
        return false;
    }

    public static bool operator ==(P2Int obj1, P2Int obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P2Int obj1, P2Int obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P2Int operator +(P2Int p1, P2Int p2)
    {
        return p1.Shift(p2);
    }

    public static P2Int operator -(P2Int p1, P2Int p2)
    {
        return new P2Int(p1._x - p2._x, p1._y - p2._y);
    }

    public static P2Int operator -(P2Int p1)
    {
        return new P2Int(-p1._x, -p1._y);
    }

    public static P2Int operator *(P2Int p1, int num)
    {
        return new P2Int(p1._x * num, p1._y * num);
    }
}