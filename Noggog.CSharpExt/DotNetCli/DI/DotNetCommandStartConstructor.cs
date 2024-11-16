using System.Diagnostics;

namespace Noggog.DotNetCli.DI;

public interface IDotNetCommandStartConstructor
{
    ProcessStartInfo Construct(string command, FilePath path, params string?[] args);
}

public class DotNetCommandStartConstructor : IDotNetCommandStartConstructor
{
    public IDotNetCommandPathProvider DotNetPathProvider { get; }

    public DotNetCommandStartConstructor(
        IDotNetCommandPathProvider dotNetPathProvider)
    {
        DotNetPathProvider = dotNetPathProvider;
    }

    public ProcessStartInfo Construct(string command, FilePath path, params string?[] args)
    {
        var argStr = string.Join(" ", args.WhereNotNull());
        var cmd =  $"{command} \"{path.RelativePath}\"{(argStr.IsNullOrWhitespace() ? string.Empty : $" {argStr}")}";
        return new ProcessStartInfo(DotNetPathProvider.Path, cmd);
    }
}