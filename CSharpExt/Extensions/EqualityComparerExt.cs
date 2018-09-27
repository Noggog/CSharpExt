using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EqualityComparerExt<T>
    {
        public class ReferenceEqualityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return EqualityComparer<T>.ReferenceEquals(x, y);
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public static readonly ReferenceEqualityComparer ReferenceEquality = new ReferenceEqualityComparer();
    }
}
