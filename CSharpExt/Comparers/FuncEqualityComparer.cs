using System;
using System.Collections.Generic;

namespace Noggog
{
    public class FuncEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _hash;

        public FuncEqualityComparer(
            Func<T, T, bool> equals,
            Func<T, int> hash)
        {
            this._equals = equals;
            this._hash = hash;
        }

        public FuncEqualityComparer(Func<T, T, bool> equals)
        {
            this._equals = equals;
        }

        public FuncEqualityComparer(Func<T, int> hash)
        {
            this._hash = hash;
        }

        public override bool Equals(T x, T y)
        {
            return this._equals(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return this._hash(obj);
        }
    }
}