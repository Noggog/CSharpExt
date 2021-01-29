using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct HashableDecimal : IEquatable<HashableDecimal>, IComparable<HashableDecimal>
    {
        public const int NumDecimals = 9;
        private static readonly decimal Pow = (decimal)Math.Pow(10, NumDecimals);
        private readonly int _integerValue;
        private readonly int _decimalValue;
        private readonly int _hash;

        /// <summary>
        /// Decimal value.  Calculated on demand per access
        /// </summary>
        public decimal Value => GetDecimal();

        public HashableDecimal(decimal d)
        {
            bool positive = d > 0;
            _integerValue = positive ? (int)Math.Floor(d) : (int)Math.Ceiling(d);
            d -= _integerValue;
            d *= Pow;
            _decimalValue = positive ? (int)Math.Floor(d) : (int)Math.Ceiling(d);

            // Construct and cache hash up front.
            // We could also not do this, if we felt the upfront cost didn't match our usage patterns
            HashCode h = new HashCode();
            h.Add(_integerValue);
            h.Add(_decimalValue);
            _hash = h.ToHashCode();
        }

        public override bool Equals(object? obj)
        {
            // Could modify to handle doubles/decimals/etc if we wanted
            if (obj is not HashableDecimal p) return false;
            return Equals(p);
        }

        public static bool operator ==(HashableDecimal p1, HashableDecimal p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(HashableDecimal p1, HashableDecimal p2)
        {
            return !p1.Equals(p2);
        }

        public static bool operator <(HashableDecimal p1, HashableDecimal p2)
        {
            return p1.CompareTo(p2) < 0;
        }

        public static bool operator >(HashableDecimal p1, HashableDecimal p2)
        {
            return p1.CompareTo(p2) > 0;
        }

        public static bool operator <=(HashableDecimal p1, HashableDecimal p2)
        {
            return p1.CompareTo(p2) <= 0;
        }

        public static bool operator >=(HashableDecimal p1, HashableDecimal p2)
        {
            return p1.CompareTo(p2) >= 0;
        }

        public static implicit operator decimal(HashableDecimal p)
        {
            return p.Value;
        }

        public override int GetHashCode() => _hash;

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(string format)
        {
            return Value.ToString(format);
        }

        public bool Equals(HashableDecimal other)
        {
            if (_integerValue != other._integerValue) return false;
            if (_decimalValue != other._decimalValue) return false;
            return true;
        }

        private decimal GetDecimal()
        {
            return _integerValue + (_decimalValue / Pow);
        }

        public int CompareTo(HashableDecimal other)
        {
            var comp = _integerValue.CompareTo(other._integerValue);
            if (comp != 0) return comp;
            comp = _decimalValue.CompareTo(other._decimalValue);
            if (comp != 0) return comp;
            return 0;
        }
    }
}
