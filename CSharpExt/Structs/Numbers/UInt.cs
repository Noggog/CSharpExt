using System;

namespace Noggog
{
    public struct UInt : IComparable<int>, IEquatable<UInt>
    {
        public readonly int Value;

        public UInt(int val)
        {
            if (val < 0)
            {
                throw new ArgumentOutOfRangeException("val", val, "Value was set to below 0");
            }
            Value = val;
        }

        public static implicit operator UInt(int i)
        {
            return new UInt(i);
        }

        public static implicit operator int (UInt i)
        {
            return i.Value;
        }

        public static UInt operator ++(UInt i)
        {
            return i + 1;
        }

        public static UInt operator --(UInt i)
        {
            return i - 1;
        }

        public static bool operator <(UInt u1, UInt u2)
        {
            return u1.Value < u2.Value;
        }

        public static bool operator >(UInt u1, UInt u2)
        {
            return u1.Value > u2.Value;
        }

        public static bool operator <=(UInt u1, UInt u2)
        {
            return u1.Value <= u2.Value;
        }

        public static bool operator >=(UInt u1, UInt u2)
        {
            return u1.Value >= u2.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is UInt)
            {
                var uRhs = (UInt)obj;
                return this.Value == uRhs.Value;
            }
            if (obj is int)
            {
                return this.Value == (int)obj;
            }
            if (!(obj is long)) return false;
            try
            {
                return Math.Abs((long) obj) < int.MaxValue && Value == Convert.ToInt32(obj);
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        public bool Equals(UInt other)
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

        public int CompareTo(UInt other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(int other)
        {
            return Value.CompareTo(other);
        }
    }
}
