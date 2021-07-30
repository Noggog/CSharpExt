using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct ErrorResponse : IEquatable<ErrorResponse>
    {
        public readonly static ErrorResponse Success = Succeed();
        public readonly static ErrorResponse Failure = new ErrorResponse(succeeded: false);

        private readonly bool _failed;
        public readonly bool Succeeded => !_failed;
        private readonly Exception? _exception;
        public readonly Exception? Exception => _exception;
        private readonly string _reason;

        public bool Failed => _failed;
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

        private ErrorResponse(
            bool succeeded,
            string reason = "",
            Exception? ex = null)
        {
            this._failed = !succeeded;
            this._reason = reason;
            this._exception = ex;
        }

        public override string ToString()
        {
            return $"({(Succeeded ? "Success" : "Fail")}, {Reason})";
        }

        public GetResponse<TRet> BubbleFailure<TRet>()
        {
            if (this.Exception == null)
            {
                return GetResponse<TRet>.Fail(this.Reason);
            }
            return GetResponse<TRet>.Fail(this.Exception);
        }

        public GetResponse<TRet> BubbleResult<TRet>(TRet item)
        {
            if (this.Exception != null)
            {
                return GetResponse<TRet>.Fail(item, this.Exception);
            }
            return GetResponse<TRet>.Create(successful: this.Succeeded, val: item, reason: this.Reason);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ErrorResponse resp) return false;
            return Equals(resp);
        }

        public bool Equals(ErrorResponse other)
        {
            if (this._failed != other._failed) return false;
            if (_exception != null)
            {
                return object.Equals(_exception, other._exception);
            }
            else
            {
                return object.Equals(_reason, other._reason);
            }
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(_failed);
            hash.Add(_exception);
            hash.Add(_reason);
            return hash.ToHashCode();
        }

        #region Factories
        public static ErrorResponse Succeed()
        {
            return new ErrorResponse(true);
        }

        public static ErrorResponse Succeed(string reason)
        {
            return new ErrorResponse(true, reason);
        }

        public static ErrorResponse Fail(string reason)
        {
            return new ErrorResponse(false, reason: reason);
        }

        public static ErrorResponse Fail(Exception ex)
        {
            return new ErrorResponse(false, ex: ex);
        }

        public static ErrorResponse Fail()
        {
            return new ErrorResponse(false);
        }

        public static ErrorResponse Create(bool successful, string reason = "")
        {
            return new ErrorResponse(successful, reason);
        }
        #endregion
    }
}
