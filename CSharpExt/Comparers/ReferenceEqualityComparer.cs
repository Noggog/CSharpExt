using System;
using System.Collections.Generic;

namespace Noggog
{
    public class ReferenceEqualityComparer<T> : EqualityComparer<T>
    {
        public static ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
        }

        public override bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}