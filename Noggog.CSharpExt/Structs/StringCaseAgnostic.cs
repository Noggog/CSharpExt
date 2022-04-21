namespace Noggog;

public struct StringCaseAgnostic : IEquatable<string>, IEquatable<StringCaseAgnostic>
{
    public readonly string Upper;
    public readonly string Value;

    public StringCaseAgnostic(string str)
    {
        Value = str;
        Upper = str.ToUpper();
    }

    public StringCaseAgnostic(StringCaseAgnostic rhs)
    {
        Value = rhs.Value;
        Upper = rhs.Upper;
    }

    public override string ToString()
    {
        return Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not StringCaseAgnostic rhs) return false;
        return Equals(rhs);
    }

    public bool Equals(string? other)
    {
        return string.Equals(Upper, other?.ToUpper());
    }

    public bool Equals(StringCaseAgnostic other)
    {
        return string.Equals(Upper, other.Upper);
    }

    public override int GetHashCode() => HashCode.Combine(Upper);

    public static implicit operator string(StringCaseAgnostic ag)
    {
        return ag.Value;
    }

    public static implicit operator StringCaseAgnostic(string ag)
    {
        return new StringCaseAgnostic(ag);
    }
}