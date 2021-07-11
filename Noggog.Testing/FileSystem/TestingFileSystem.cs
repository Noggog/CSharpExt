namespace Noggog.Testing.FileSystem
{
    public static class TestingFileSystem
    {
        public static MockFileSystemWatcherFactory GetFileWatcher(out MockFileSystemWatcher watcher)
        {
            watcher = new MockFileSystemWatcher();
            return new MockFileSystemWatcherFactory(watcher);
        }
    }
}