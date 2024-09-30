namespace Noggog;

public readonly struct UDouble : IComparable<UDouble>, IComparable<double>, IEquatable<UDouble>
{
    public readonly double Value;
    public const double MinValue = 0d;
    public const double MaxValue = double.MaxValue;

    public UDouble(double val)
    {
        if (val < 0)
        {
            throw new ArgumentException("Value was less than zero.");
        }
        Value = val;
    }

    public static UDouble FactorySafe(double d)
    {
        if (d < 0)
        {
            d = 0;
        }
        return new UDouble(d);
    }

    public static implicit operator UDouble(double d)
    {
        return new UDouble(d);
    }

    public static implicit operator double(UDouble d)
    {
        return d.Value;
    }

    public static UDouble operator -(UDouble d, double amount)
    {
        if (amount > d.Value)
        {
            return 0d;
        }
        return d.Value - amount;
    }

    public static UDouble operator +(UDouble d, double amount)
    {
        return d.Value + amount;
    }

    public override bool Equals(object? obj)
    {
        if (obj is UDouble uRhs)
        {
            return Value == uRhs.Value;
        }
        else if (obj is double d)
        {
            return Value == d;
        }
        else
        {
            return false;
        }
    }

    public bool Equals(UDouble other)
    {
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public string ToString(string format)
    {
        return Value.ToString(format);
    }

    public int CompareTo(UDouble other)
    {
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(double other)
    {
        return Value.CompareTo(other);
    }

    public static UDouble Parse(string str)
    {
        if (TryParse(str, out UDouble ud))
            return ud;
        throw new ArgumentException("Could not be converted to a UDouble", nameof(str));
    }

    public static UDouble? TryParse(string str)
    {
        if (TryParse(str, out UDouble ud))
            return ud;
        return null;
    }

    public static bool TryParse(string str, out UDouble doub)
    {
        if (double.TryParse(str, out double d))
        {
            doub = new UDouble(d);
            return true;
        }
        doub = new UDouble();
        return false;
    }

    public bool EqualsWithin(UDouble rhs, double within = 0.000000001d)
    {
        return Value.EqualsWithin(rhs.Value, within);
    }

    public bool IsInRange(UDouble min, UDouble max)
    {
        if (Value < min) return false;
        if (Value > max) return false;
        return true;
    }

    public UDouble InRange(UDouble min, UDouble max)
    {
        if (Value < min) throw new ArgumentException($"{Value} was lower than the minimum {min}.");
        if (Value > max) throw new ArgumentException($"{Value} was greater than the maximum {max}.");
        return Value;
    }

    public UDouble PutInRange(UDouble min, UDouble max)
    {
        if (Value < min) return min;
        if (Value > max) return max;
        return Value;
    }

    public static bool operator ==(UDouble left, UDouble right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UDouble left, UDouble right)
    {
        return !(left == right);
    }

    public static bool operator <(UDouble left, UDouble right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(UDouble left, UDouble right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(UDouble left, UDouble right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(UDouble left, UDouble right)
    {
        return left.CompareTo(right) >= 0;
    }
}