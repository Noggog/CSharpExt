namespace Noggog.IO;

public interface ICurrentDirectoryProvider
{
    DirectoryPath CurrentDirectory { get; }
}

public class CurrentDirectoryProvider : ICurrentDirectoryProvider
{
    public DirectoryPath CurrentDirectory => Environment.CurrentDirectory;
}

public class CurrentDirectoryInjection : ICurrentDirectoryProvider
{
    public CurrentDirectoryInjection(DirectoryPath currentDirectory)
    {
        CurrentDirectory = currentDirectory;
    }

    public DirectoryPath CurrentDirectory { get; }
}