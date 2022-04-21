namespace CSharpExt.UnitTests;

public class WrappedInt : IComparable, IComparable<WrappedInt>, IEquatable<WrappedInt>
{
    public readonly int Int;

    public WrappedInt(int i)
    {
        Int = i;
    }

    public int CompareTo(WrappedInt? other)
    {
        return Int.CompareTo(other!.Int);
    }

    public int CompareTo(object? obj)
    {
        if (!(obj is WrappedInt rhs)) return 0;
        return CompareTo(rhs);
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is WrappedInt rhs)) return false;
        return Equals(rhs);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Int);
    }

    public bool Equals(WrappedInt? other)
    {
        if (other == null) return false;
        return Int == other.Int;
    }

    public override string ToString()
    {
        return Int.ToString();
    }
}