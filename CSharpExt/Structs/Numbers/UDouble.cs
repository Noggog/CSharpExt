using System;

namespace Noggog
{
    public struct UDouble : IComparable<UDouble>, IComparable<double>, IEquatable<UDouble>
    {
        public double Value;

        public UDouble(double val)
        {
            if (val < 0)
            {
                val = 0;
            }
            Value = val;
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

        public override bool Equals(object obj)
        {
            if (obj is UDouble)
            {
                UDouble uRhs = (UDouble)obj;
                return this.Value == uRhs.Value;
            }
            else if (obj is double)
            {
                return this.Value == (double)obj;
            }
            else
            {
                return false;
            }
        }

        public bool Equals(UDouble other)
        {
            return this.Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public int CompareTo(UDouble other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(double other)
        {
            return Value.CompareTo(other);
        }

        public static bool TryParse(string str, out UDouble doub)
        {
            if (!double.TryParse(str, out double d))
            {
                doub = new UDouble();
                return false;
            }
            doub = new UDouble(d);
            return true;
        }
    }
}
