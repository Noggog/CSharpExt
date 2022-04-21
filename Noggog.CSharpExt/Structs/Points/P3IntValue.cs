using System.Runtime.Serialization;

namespace Noggog;

public class P3IntValueObj<T>
{
    public P3IntValue<T> Value;

    public static implicit operator P3IntValue<T>(P3IntValueObj<T> obj)
    {
        return obj.Value;
    }

    public static implicit operator P3IntValueObj<T>(P3IntValue<T> obj)
    {
        return new P3IntValueObj<T>() { Value = obj };
    }
}

public struct P3IntValue<T> : IP3IntGet, IEquatable<P3IntValue<T>>
{
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
        
    private T _value;
    [DataMember]
    public T Value
    {
        get => _value;
        set => _value = value;
    }
        
    [IgnoreDataMember]
    P3Int IP3IntGet.Point => new(X, Y, Z);

    public P3IntValue(int x, int y, int z, T val)
    {
        _x = x;
        _y = y;
        _z = z;
        _value = val;
    }

    public P3IntValue(P3Int rhs, T val)
    {
        _x = rhs.X;
        _y = rhs.Y;
        _z = rhs.Z;
        _value = val;
    }

    public P3IntValue(P3IntValue<T> rhs)
    {
        _x = rhs._x;
        _y = rhs._y;
        _z = rhs._z;
        _value = rhs.Value;
    }

    public override string ToString()
    {
        return $"({_x},{_y},{_z},{_value})";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not P3IntValue<T> rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(P3IntValue<T> rhs)
    {
        return _x == rhs._x
               && _y == rhs._y
               && _z == rhs._z
               && Equals(_value, rhs._value);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(_x);
        hash.Add(_y);
        hash.Add(_z);
        hash.Add(_value);
        return hash.ToHashCode();
    }
        
    public static bool operator ==(P3IntValue<T> left, P3IntValue<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(P3IntValue<T> left, P3IntValue<T> right)
    {
        return !Equals(left, right);
    }

    public static implicit operator P3Int(P3IntValue<T> p)
    {
        return new P3Int(p._x, p._y, p._z);
    }
}