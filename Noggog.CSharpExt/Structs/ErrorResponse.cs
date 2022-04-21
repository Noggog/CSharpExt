namespace Noggog;

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
            if (Exception != null)
            {
                return Exception.ToString();
            }
            return _reason;
        }
    }

    private ErrorResponse(
        bool succeeded,
        string reason = "",
        Exception? ex = null)
    {
        _failed = !succeeded;
        _reason = reason;
        _exception = ex;
    }

    public override string ToString()
    {
        return $"({(Succeeded ? "Success" : "Fail")}, {Reason})";
    }

    public GetResponse<TRet> BubbleFailure<TRet>()
    {
        if (Exception == null)
        {
            return GetResponse<TRet>.Fail(Reason);
        }
        return GetResponse<TRet>.Fail(Exception);
    }

    public GetResponse<TRet> BubbleResult<TRet>(TRet item)
    {
        if (Exception != null)
        {
            return GetResponse<TRet>.Fail(item, Exception);
        }
        return GetResponse<TRet>.Create(successful: Succeeded, val: item, reason: Reason);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ErrorResponse resp) return false;
        return Equals(resp);
    }

    public bool Equals(ErrorResponse other)
    {
        if (_failed != other._failed) return false;
        if (_exception != null)
        {
            return Equals(_exception, other._exception);
        }
        else
        {
            return Equals(_reason, other._reason);
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

    public static ErrorResponse Create(bool successful, string reason = "", Exception? ex = null)
    {
        return new ErrorResponse(successful, reason, ex);
    }
    #endregion
}