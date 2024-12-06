namespace Noggog.Nuget.Errors;

public interface INugetErrorSolution
{
    string ErrorText { get; }
    void RunFix(FilePath path);
}