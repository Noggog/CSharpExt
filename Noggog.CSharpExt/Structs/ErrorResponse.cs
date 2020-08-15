using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct ErrorResponse : IErrorResponse
    {
        public readonly static ErrorResponse Success = Succeed();
        public readonly static ErrorResponse Failure = new ErrorResponse();

        private readonly bool _succeeded;
        public readonly bool Succeeded => _succeeded;
        private readonly Exception? _exception;
        public readonly Exception? Exception => _exception;
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

        bool IErrorResponse.Succeeded => this.Succeeded;
        Exception? IErrorResponse.Exception => this.Exception;

        private ErrorResponse(
            bool succeeded,
            string reason = "",
            Exception? ex = null)
        {
            this._succeeded = succeeded;
            this._reason = reason;
            this._exception = ex;
        }

        public override string ToString()
        {
            return $"({(Succeeded ? "Success" : "Fail")}, {Reason})";
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

        public static ErrorResponse Convert(IErrorResponse err, bool nullIsSuccess = true)
        {
            if (err == null) return Create(nullIsSuccess);
            return new ErrorResponse(err.Succeeded, err.Reason, err.Exception);
        }
    }

    public interface IErrorResponse
    {
        bool Succeeded { get; }
        Exception? Exception { get; }
        string Reason { get; }
    }

    public static class ErrorResponseExt
    {
        public static GetResponse<TRet> BubbleFailure<TRet>(this IErrorResponse resp)
        {
            if (resp.Exception == null)
            {
                return GetResponse<TRet>.Fail(resp.Reason);
            }
            return GetResponse<TRet>.Fail(resp.Exception);
        }
    }
}
