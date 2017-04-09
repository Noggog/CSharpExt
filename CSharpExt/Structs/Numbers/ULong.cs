using System;

namespace Noggog
{
    public struct ULong : IComparable<long>, IEquatable<ULong>
    {
        public readonly long Value;

        public ULong(long val)
        {
            if (val < 0)
            {
                throw new ArgumentOutOfRangeException("val", val, "Value was set to below 0");
            }
            Value = val;
        }

        public static implicit operator ULong(long i)
        {
            return new ULong(i);
        }

        public static implicit operator long (ULong i)
        {
            return i.Value;
        }

        public static implicit operator ULong(UInt i)
        {
            return new ULong(i);
        }

        public static implicit operator UInt(ULong i)
        {
            if (Math.Abs(i.Value) < int.MaxValue)
            {
                return new UInt(Convert.ToInt32(i.Value));
            }
            throw new ArgumentOutOfRangeException("i", i.Value, "Out of range conversion of long to int");
        }

        public static ULong operator ++(ULong i)
        {
            return i + 1;
        }

        public static ULong operator --(ULong i)
        {
            return i - 1;
        }

        public override bool Equals(object obj)
        {
            if (obj is ULong uRhs)
            {
                return this.Value == uRhs.Value;
            }
            if (obj is long)
            {
                return Value == (long)obj;
            }
            if (obj is int)
            {
                return Value == Convert.ToInt64(obj);
            }
            return false;
        }

        public bool Equals(ULong other)
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

        public int CompareTo(ULong other)
        {
            return Value.CompareTo(other.Value);
        }

        public int CompareTo(long other)
        {
            return Value.CompareTo(other);
        }
    }
}
