using Noggog;
using System;
using System.Diagnostics;

namespace Noggog
{
    public struct TryGet<T> : IEquatable<TryGet<T>>
    {
        public readonly static TryGet<T> Failure = new TryGet<T>();
        
        public readonly T Value;
        public readonly bool Succeeded;
        public bool Failed => !Succeeded;

        private TryGet(
            bool succeeded,
            T val = default(T))
        {
            this.Value = val;
            this.Succeeded = succeeded;
        }

        public bool Equals(TryGet<T> other)
        {
            return this.Succeeded == other.Succeeded
                && object.Equals(this.Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TryGet<T> rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Value)
                .CombineHashCode(Succeeded.GetHashCode());
        }

        public override string ToString()
        {
            return $"({Succeeded}, {Value})";
        }

        public TryGet<R> Bubble<R>(Func<T, R> conv, bool fireIfFailed = false)
        {
            R val;
            if (this.Succeeded || fireIfFailed)
            {
                val = conv(this.Value);
            }
            else
            {
                val = default(R);
            }
            return TryGet<R>.Create(
                this.Succeeded,
                val);
        }

        public TryGet<R> BubbleFailure<R>()
        {
            return new TryGet<R>(false);
        }

        public T GetOrDefault(T def)
        {
            if (this.Succeeded)
            {
                return this.Value;
            }
            return def;
        }

        #region Factories
        [DebuggerStepThrough]
        public static TryGet<T> Succeed(T value)
        {
            return new TryGet<T>(true, value);
        }

        [DebuggerStepThrough]
        public static TryGet<T> Fail(T val)
        {
            return new TryGet<T>(false, val);
        }

        [DebuggerStepThrough]
        public static TryGet<T> Create(bool successful, T val = default(T))
        {
            return new TryGet<T>(successful, val);
        }
        #endregion
    }
}

namespace System
{
    public static class TryGetExt
    {
        public static R GetOrDefault<T, R>(this TryGet<T> tryGet, R def)
            where T : R
        {
            if (tryGet.Succeeded)
            {
                return tryGet.Value;
            }
            return def;
        }
    }
}
