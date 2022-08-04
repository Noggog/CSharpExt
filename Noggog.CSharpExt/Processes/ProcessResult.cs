namespace Noggog.Processes;

public record ProcessResult(int Result, List<string> Out, List<string> Errors)
{
    public bool FailedReturn => Result != 0;
    public bool FailedReturnOrErrorMessages => FailedReturn || Errors.Count > 0;
    public bool NoDetectableErrors => !FailedReturnOrErrorMessages;
    
    public ProcessResult()
        : this(-1, new(), new())
    {
    }

    public ErrorResponse AsErrorResponse()
    {
        if (Result == 0 && Errors.Count == 0)
        {
            return ErrorResponse.Success;
        }

        if (Errors.Count > 1)
        {
            return ErrorResponse.Fail($"[{Result}]: \n{Errors.First()}\n  ...");
        }
        else if (Errors.Count == 1)
        {
            return ErrorResponse.Fail($"[{Result}]: \n{Errors.First()}");
        }
        else
        {
            return ErrorResponse.Fail($"[{Result}]");
        }
    }
}
