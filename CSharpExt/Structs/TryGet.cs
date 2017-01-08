using System;

namespace System
{
    public struct TryGet<T> : IEquatable<TryGet<T>>
    {
        public readonly T Value;
        public readonly bool Succeeded;
        public bool Failed { get { return !Succeeded; } }
        public readonly string Reason;

        private TryGet(
            bool succeeded,
            T val = default(T),
            string reason = null)
        {
            this.Value = val;
            this.Succeeded = succeeded;
            this.Reason = reason;
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
            return $"TryGetResult({(Succeeded ? "Success" : "Fail")}, {Value}, {Reason})";
        }

        public TryGet<R> BubbleFailure<R>()
        {
            return new TryGet<R>(false, reason: this.Reason);
        }

        public TryGet<R> Bubble<R>(Func<T, R> conv)
        {
            return new TryGet<R>(
                this.Succeeded,
                conv(this.Value),
                this.Reason);
        }

        #region Factories
        public static TryGet<T> Success(T value)
        {
            return new TryGet<T>(true, value);
        }

        public static TryGet<T> Success(T value, string reason)
        {
            return new TryGet<T>(true, value, reason);
        }

        public static TryGet<T> Failure()
        {
            return new TryGet<T>(false);
        }

        public static TryGet<T> Failure(string reason)
        {
            return new TryGet<T>(false, reason: reason);
        }

        public static TryGet<T> Failure(T val, string reason)
        {
            return new TryGet<T>(false, val, reason);
        }

        public static TryGet<T> Failure(T val)
        {
            return new TryGet<T>(false, val);
        }

        public static TryGet<T> Create(bool successful, T val = default(T), string reason = null)
        {
            return new TryGet<T>(successful, val, reason);
        }
        #endregion
    }
}
