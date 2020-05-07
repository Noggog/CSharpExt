using System;
using System.Collections.Generic;

namespace Noggog
{
    /// <summary>
    /// An equality comparer that evaluates equality based on ReferenceEquals
    /// </summary>
    public class ReferenceEqualityComparer<T> : EqualityComparer<T>
    {
        /// <summary>
        /// A static readonly singleton instance of ReferenceEqualityComparer for use
        /// </summary>
        public readonly static ReferenceEqualityComparer<T> Instance = new ReferenceEqualityComparer<T>();

        private ReferenceEqualityComparer()
        {
        }

        /// <summary>
        /// Evaluates reference equality
        /// </summary>
        /// <param name="lhs">Left hand side</param>
        /// <param name="rhs">Right hand side</param>
        /// <returns>True if same referencec</returns>
        public override bool Equals(T lhs, T rhs)
        {
            return ReferenceEquals(lhs, rhs);
        }

        /// <summary>
        /// Calculates hash normally by calling an object's own GetHashCode
        /// </summary>
        /// <param name="obj">Object to hash</param>
        /// <returns>Hash returned by object</returns>
        public override int GetHashCode(T obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}
