namespace Noggog.DotNetCli.DI;

public interface IDotNetCommandPathProvider
{
    string Path { get; }
}

public class DefaultDotNetCommandPathProvider : IDotNetCommandPathProvider
{
    public string Path => "dotnet";
}
