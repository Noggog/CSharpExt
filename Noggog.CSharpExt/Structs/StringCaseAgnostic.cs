using System;

namespace Noggog
{
    public struct StringCaseAgnostic : IEquatable<string>, IEquatable<StringCaseAgnostic>
    {
        public readonly string Upper;
        public readonly string Value;

        public StringCaseAgnostic(string str)
        {
            this.Value = str;
            this.Upper = str.ToUpper();
        }

        public StringCaseAgnostic(StringCaseAgnostic rhs)
        {
            this.Value = rhs.Value;
            this.Upper = rhs.Upper;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StringCaseAgnostic rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(string other)
        {
            return object.Equals(this.Upper, other.ToUpper());
        }

        public bool Equals(StringCaseAgnostic other)
        {
            return object.Equals(this.Upper, other.Upper);
        }

        public override int GetHashCode() => HashCode.Combine(this.Upper);

        public static implicit operator string(StringCaseAgnostic ag)
        {
            return ag.Value;
        }

        public static implicit operator StringCaseAgnostic(string ag)
        {
            return new StringCaseAgnostic(ag);
        }
    }
}
