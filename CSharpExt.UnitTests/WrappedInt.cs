using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpExt.Tests
{
    public class WrappedInt : IComparable, IComparable<WrappedInt>, IEquatable<WrappedInt>
    {
        public readonly int Int;

        public WrappedInt(int i)
        {
            this.Int = i;
        }

        public int CompareTo(WrappedInt other)
        {
            return Int.CompareTo(other.Int);
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
            return Int.GetHashCode();
        }

        public bool Equals(WrappedInt other)
        {
            return Int == other.Int;
        }

        public override string ToString()
        {
            return Int.ToString();
        }
    }
}
