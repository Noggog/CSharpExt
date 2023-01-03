using System.IO.Abstractions;

namespace Noggog.Testing.FileSystem;

public static class TestingFileSystem
{
    public static MockFileSystemWatcherFactory GetFileWatcher(IFileSystem fileSystem, out MockFileSystemWatcher watcher)
    {
        watcher = new MockFileSystemWatcher(fileSystem);
        return new MockFileSystemWatcherFactory(fileSystem, watcher);
    }
}