using System.Runtime.Serialization;

namespace Noggog;

public struct P3Float : IEquatable<P3Float>
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
        
    private float _z;
    [DataMember]
    public float Z
    {
        get => _z;
        set => _z = value;
    }
        
    [IgnoreDataMember]
    public float Length => (float)Math.Sqrt(_x * _x + _y * _y + _z * _z);
    [IgnoreDataMember]
    public float Magnitude => Length;
    [IgnoreDataMember]
    public float SqrMagnitude => (_x * _x + _y * _y);

    [IgnoreDataMember]
    public P3Float Normalized
    {
        get
        {
            float length = Length;
            return new P3Float(_x / length, _y / length, _z / length);
        }
    }

    [IgnoreDataMember]
    public P3Float Absolute => new P3Float(
        Math.Abs(_x),
        Math.Abs(_y),
        Math.Abs(_z));

    public P3Float(float x, float y, float z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public override string ToString()
    {
        return $"({_x}, {_y}, {_z})";
    }

    public P3Float Normalize()
    {
        var length = Length;
        return new P3Float(
            _x / length,
            _y / length,
            _z / length);
    }

    public static float Dot(P3Float v1, P3Float v2) => v1._x * v2._x + v1._y * v2._y + v1._z * v2._z;
    public float Distance(P3Float p2) => (this - p2).Magnitude;

    public static bool TryParse(string str, out P3Float p2)
    {
        string[] split = str.Split(',');
        if (split.Length != 3)
        {
            p2 = default(P3Float);
            return false;
        }

        if (!float.TryParse(split[0], out float x))
        {
            p2 = default(P3Float);
            return false;
        }
        if (!float.TryParse(split[1], out float y))
        {
            p2 = default(P3Float);
            return false;
        }
        if (!float.TryParse(split[2], out float z))
        {
            p2 = default(P3Float);
            return false;
        }
        p2 = new P3Float(x, y, z);
        return true;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P3Float rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3Float rhs)
    {
        return _x.EqualsWithin(rhs._x)
               && _y.EqualsWithin(rhs._y)
               && _z.EqualsWithin(rhs._z);
    }

    public override int GetHashCode() => HashCode.Combine(_x, _y, _z);

    public static P3Float Max(P3Float p, P3Float c)
    {
        return new P3Float(Math.Max(p._x, c._x), Math.Max(p._y, c._y), Math.Max(p._z, c._z));
    }

    public P3Float Max(float c)
    {
        return new P3Float(Math.Max(_x, c), Math.Max(_y, c), Math.Max(_z, c));
    }

    public static bool operator ==(P3Float obj1, P3Float obj2)
    {
        return obj1.Equals(obj2);
    }

    public static bool operator !=(P3Float obj1, P3Float obj2)
    {
        return !obj1.Equals(obj2);
    }

    public static P3Float operator -(P3Float c1)
    {
        return new P3Float(-c1._x, -c1._y, -c1._z);
    }

    public static P3Float operator +(P3Float c1, P3Float c2)
    {
        return new P3Float(c1._x + c2._x, c1._y + c2._y, c1._z + c2._z);
    }

    public static P3Float operator +(P3Float c1, float f)
    {
        return new P3Float(c1._x + f, c1._y + f, c1._y + f);
    }

    public static P3Float operator -(P3Float c1, P3Float c2)
    {
        return new P3Float(c1._x - c2._x, c1._y - c2._y, c1._z - c2._z);
    }

    public static P3Float operator -(P3Float c1, float f)
    {
        return new P3Float(c1._x - f, c1._y - f, c1._z - f);
    }

    public static P3Float operator *(P3Float c1, P3Float c2)
    {
        return new P3Float(c1._x * c2._x, c1._y * c2._y, c1._z * c2._z);
    }

    public static P3Float operator *(P3Float c1, float f)
    {
        return new P3Float(c1._x * f, c1._y * f, c1._z * f);
    }

    public static P3Float operator /(P3Float c1, P3Float c2)
    {
        return new P3Float(c1._x / c2._x, c1._y / c2._y, c1._z / c2._z);
    }

    public static P3Float operator /(P3Float c1, float f)
    {
        return new P3Float(c1._x / f, c1._y / f, c1._z / f);
    }

    public static implicit operator P3Float(P3Int point)
    {
        return new P3Float(point.X, point.Y, point.Z);
    }
}