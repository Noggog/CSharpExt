using Noggog;
using System;
using System.Diagnostics;

namespace Noggog
{
    public interface ITryGetter<out T>
    {
        T Value { get; }
        bool Succeeded { get; }
        bool Failed { get; }
    }

    public struct TryGet<T> : IEquatable<TryGet<T>>, ITryGetter<T>
    {
        public readonly static TryGet<T> Failure = new TryGet<T>();
        
        public readonly T Value;
        public readonly bool Succeeded;
        public bool Failed => !Succeeded;

        T ITryGetter<T>.Value => this.Value;
        bool ITryGetter<T>.Succeeded => this.Succeeded;

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
            var hash = new HashCode();
            hash.Add(Value);
            hash.Add(Succeeded);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return $"({Succeeded}, {Value})";
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

namespace Noggog
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
