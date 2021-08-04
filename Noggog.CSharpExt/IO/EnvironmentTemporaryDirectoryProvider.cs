namespace Noggog.IO
{
    public interface IEnvironmentTemporaryDirectoryProvider
    {
        DirectoryPath Path { get; }
    }

    public class EnvironmentTemporaryDirectoryProvider : IEnvironmentTemporaryDirectoryProvider
    {
        public DirectoryPath Path => System.IO.Path.GetTempPath();
    }
}