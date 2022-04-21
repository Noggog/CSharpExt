namespace Noggog.Utility.ArgsQuerying;

public class ArgsQueryConsoleManager : ArgsQueryManager
{
    public ArgsQueryConsoleManager(string[] args) 
        : base(PromptSnippet, Read, args)
    {
    }

    public ArgsQueryConsoleManager(IEnumerable<string> args) 
        : base(PromptSnippet, Read, args)
    {
    }

    private static void PromptSnippet(string prompt)
    {
        Console.WriteLine(prompt);
    }

    private static string Read()
    {
        return Console.ReadLine() ?? throw new ArgumentNullException();
    }
}