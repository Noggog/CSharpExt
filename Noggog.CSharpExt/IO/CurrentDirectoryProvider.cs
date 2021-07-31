using System;

namespace Noggog.IO
{
    public interface ICurrentDirectoryProvider
    {
        DirectoryPath CurrentDirectory { get; }
    }

    public class CurrentDirectoryProvider : ICurrentDirectoryProvider
    {
        public DirectoryPath CurrentDirectory => Environment.CurrentDirectory;
    }
}