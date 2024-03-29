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
            T? val = default(T))
        {
            Value = val!;
            Succeeded = succeeded;
        }

        public bool Equals(TryGet<T> other)
        {
            return Succeeded == other.Succeeded
                && Equals(Value, other.Value);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not TryGet<T> rhs) return false;
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
            if (Succeeded)
            {
                return Value;
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
        public static TryGet<T> Create(bool successful, T? val = default(T))
        {
            return new TryGet<T>(successful, val!);
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
