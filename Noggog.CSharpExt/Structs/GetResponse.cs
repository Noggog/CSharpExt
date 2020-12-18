using System;

namespace Noggog
{
    public struct GetResponse<T> : IEquatable<GetResponse<T>>
    {
        public static readonly GetResponse<T> Failure = new GetResponse<T>();

        public T Value { get; }
        public bool Succeeded { get; }
        public Exception? Exception { get; }
        private readonly string _reason;

        public bool Failed => !Succeeded;
        public string Reason
        {
            get
            {
                if (this.Exception != null)
                {
                    return this.Exception.ToString();
                }
                return _reason;
            }
        }

        private GetResponse(
            bool succeeded,
            T val = default,
            string reason = "",
            Exception? ex = null)
        {
            this.Value = val!;
            this.Succeeded = succeeded;
            this._reason = reason;
            this.Exception = ex;
        }

        public bool Equals(GetResponse<T> other)
        {
            return this.Succeeded == other.Succeeded
                && object.Equals(this.Value, other.Value)
                && string.Equals(_reason, other.Reason)
                && object.Equals(Exception, other.Exception);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not GetResponse<T> rhs) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Value);
            hash.Add(Succeeded);
            hash.Add(_reason);
            hash.Add(Exception);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return $"({(Succeeded ? "Success" : "Fail")}, {Value}, {Reason})";
        }

        public GetResponse<R> BubbleFailure<R>()
        {
            return new GetResponse<R>(
                succeeded: false, 
                reason: this._reason, 
                ex: this.Exception);
        }

        public GetResponse<R> Bubble<R>(Func<T, R> conv)
        {
            return new GetResponse<R>(
                succeeded: this.Succeeded,
                val: conv(this.Value),
                reason: this._reason,
                ex: this.Exception);
        }

        public T EvaluateOrThrow()
        {
            if (this.Succeeded)
            {
                return this.Value;
            }
            throw new ArgumentException(this.Reason);
        }

        public static implicit operator GetResponse<T>(T item)
        {
            return GetResponse<T>.Succeed(item);
        }

        public static implicit operator ErrorResponse(GetResponse<T> item)
        {
            if (item.Exception != null) return ErrorResponse.Fail(item.Exception);
            return ErrorResponse.Create(successful: item.Succeeded, reason: item.Reason);
        }

        #region Factories
        public static GetResponse<T> Succeed(T value)
        {
            return new GetResponse<T>(true, value);
        }

        public static GetResponse<T> Succeed(T value, string reason)
        {
            return new GetResponse<T>(true, value, reason);
        }

        public static GetResponse<T> Fail(string reason)
        {
            return new GetResponse<T>(false, reason: reason);
        }

        public static GetResponse<T> Fail(T val, string reason)
        {
            return new GetResponse<T>(false, val, reason);
        }

        public static GetResponse<T> Fail(Exception ex)
        {
            return new GetResponse<T>(false, ex: ex);
        }

        public static GetResponse<T> Fail(T val, Exception ex)
        {
            return new GetResponse<T>(false, val, ex: ex);
        }

        public static GetResponse<T> Fail(T val)
        {
            return new GetResponse<T>(false, val);
        }

        public static GetResponse<T> Create(bool successful, T val = default, string reason = "")
        {
            return new GetResponse<T>(successful, val!, reason);
        }
        #endregion
    }
}
