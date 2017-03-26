using System;

namespace Noggog
{
    public struct TryGet<T> : IEquatable<TryGet<T>>
    {
        public readonly static TryGet<T> Failure = new TryGet<T>();
        
        public readonly T Value;
        public readonly bool Succeeded;
        public bool Failed { get { return !Succeeded; } }

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
            if (!(obj is TryGet<T>)) return false;
            return Equals((TryGet<T>)obj);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Value)
                .CombineHashCode(Succeeded.GetHashCode());
        }

        public override string ToString()
        {
            return $"TryGetResult({Succeeded}, {Value})";
        }

        public TryGet<R> Bubble<R>(Func<T, R> conv)
        {
            return TryGet<R>.Create(
                this.Succeeded,
                conv(this.Value));
        }

        public TryGet<R> BubbleFailure<R>()
        {
            return new TryGet<R>(false);
        }

        #region Factories
        public static TryGet<T> Succeed(T value)
        {
            return new TryGet<T>(true, value);
        }
        
        public static TryGet<T> Fail(T val)
        {
            return new TryGet<T>(false, val);
        }

        public static TryGet<T> Create(bool successful, T val = default(T))
        {
            return new TryGet<T>(successful, val);
        }
        #endregion
    }
}
