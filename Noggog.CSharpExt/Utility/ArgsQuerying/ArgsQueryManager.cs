namespace Noggog.Utility.ArgsQuerying;

public class ArgsQueryManager
{
    List<string> argsColl;
    int nextIndex;
    Action<string> outputAction;
    Func<string> inputAction;

    public ArgsQueryManager(
        Action<string> outputAction,
        Func<string> inputAction,
        string[] args)
        : this(outputAction, inputAction, (IEnumerable<string>)args)
    {
    }

    public ArgsQueryManager(
        Action<string> outputAction,
        Func<string> inputAction,
        IEnumerable<string> args)
    {
        argsColl = new List<string>(args);
        this.outputAction = outputAction;
        this.inputAction = inputAction;
    }

    public string Prompt(string queryMessage)
    {
        if (argsColl.Count > nextIndex)
        {
            return argsColl[nextIndex++];
        }

        outputAction(queryMessage);
        return inputAction();
    }

    public bool PromptBool(string queryMessage)
    {
        var resp = Prompt(queryMessage);
        resp = resp.ToUpper();
        switch (resp)
        {
            case "YES":
            case "TRUE":
            case "T":
            case "1":
                return true;
            case "NO":
            case "FALSE":
            case "F":
            case "0":
                return false;
            default:
                outputAction("Try again.");
                return PromptBool(queryMessage);
        }
    }
}