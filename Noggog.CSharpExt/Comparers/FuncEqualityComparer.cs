using System;
using System.Collections.Generic;

namespace Noggog
{
    /// <summary>
    /// An equality comparer that uses custom Funcs to fulfill Equals and Hash requests.
    /// </summary>
    /// <typeparam name="T">Type of object being compared</typeparam>
    public class FuncEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _hash;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="equals">Func to call for Equals requests</param>
        /// <param name="hash">Func to call for Hash requests</param>
        /// <returns>True if func returned true</returns>
        public FuncEqualityComparer(
            Func<T, T, bool> equals,
            Func<T, int> hash)
        {
            this._equals = equals;
            this._hash = hash;
        }

        /// <summary>
        /// Evaluates equality by calling internal func
        /// </summary>
        /// <param name="lhs">Left hand side</param>
        /// <param name="rhs">Right hand side</param>
        /// <returns>Equality result returned by func</returns>
        public override bool Equals(T lhs, T rhs)
        {
            return this._equals(lhs, rhs);
        }

        /// <summary>
        /// Calculates hash by calling internal func
        /// </summary>
        /// <param name="obj">Object to hash</param>
        /// <returns>Hash returned by func</returns>
        public override int GetHashCode(T obj)
        {
            return this._hash(obj);
        }
    }
}
