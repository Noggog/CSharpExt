using System;

namespace System
{
    public struct StringCaseAgnostic : IEquatable<string>, IEquatable<StringCaseAgnostic>
    {
        private string upper;
        public string Upper { get { return upper; } }
        public string Value;

        public StringCaseAgnostic(string str)
        {
            Value = str;
            upper = str == null ? null : str.ToUpper();
        }

        public StringCaseAgnostic(StringCaseAgnostic rhs)
        {
            Value = rhs.Value;
            upper = rhs.upper;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is StringCaseAgnostic)
            {
                return this.Equals((StringCaseAgnostic)obj);
            }
            return Equals(obj.ToString());
        }

        public bool Equals(string other)
        {
            return this.upper.Equals(other.ToUpper());
        }

        public bool Equals(StringCaseAgnostic other)
        {
            return this.upper.Equals(other.upper);
        }

        public override int GetHashCode()
        {
            return upper.GetHashCode();
        }

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
