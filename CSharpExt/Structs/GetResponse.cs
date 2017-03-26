using System;

namespace Noggog
{
    public struct GetResponse<T> : IEquatable<GetResponse<T>>
    {
        public readonly T Value;
        public readonly bool Succeeded;
        public bool Failed { get { return !Succeeded; } }
        public readonly string Reason;

        private GetResponse(
            bool succeeded,
            T val = default(T),
            string reason = null)
        {
            this.Value = val;
            this.Succeeded = succeeded;
            this.Reason = reason;
        }

        public bool Equals(GetResponse<T> other)
        {
            return this.Succeeded == other.Succeeded
                && object.Equals(this.Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GetResponse<T>)) return false;
            return Equals((GetResponse<T>)obj);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(Value)
                .CombineHashCode(Succeeded.GetHashCode());
        }

        public override string ToString()
        {
            return $"GetResponse({(Succeeded ? "Success" : "Fail")}, {Value}, {Reason})";
        }

        public GetResponse<R> BubbleFailure<R>()
        {
            return new GetResponse<R>(false, reason: this.Reason);
        }

        public GetResponse<R> Bubble<R>(Func<T, R> conv)
        {
            return new GetResponse<R>(
                this.Succeeded,
                conv(this.Value),
                this.Reason);
        }

        public T EvaluateOrThrow()
        {
            if (this.Succeeded)
            {
                return this.Value;
            }
            throw new ArgumentException(this.Reason);
        }

        #region Factories
        public static GetResponse<T> Success(T value)
        {
            return new GetResponse<T>(true, value);
        }

        public static GetResponse<T> Success(T value, string reason)
        {
            return new GetResponse<T>(true, value, reason);
        }

        public static GetResponse<T> Failure()
        {
            return new GetResponse<T>(false);
        }

        public static GetResponse<T> Failure(string reason)
        {
            return new GetResponse<T>(false, reason: reason);
        }

        public static GetResponse<T> Failure(T val, string reason)
        {
            return new GetResponse<T>(false, val, reason);
        }

        public static GetResponse<T> Failure(T val)
        {
            return new GetResponse<T>(false, val);
        }

        public static GetResponse<T> Create(bool successful, T val = default(T), string reason = null)
        {
            return new GetResponse<T>(successful, val, reason);
        }
        #endregion
    }
}
