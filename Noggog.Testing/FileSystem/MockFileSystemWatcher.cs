using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Noggog.Testing.FileSystem;

public class MockFileSystemWatcherFactory : IFileSystemWatcherFactory
{
    private readonly MockFileSystemWatcher _mock;
    public IFileSystem FileSystem { get; }

    public MockFileSystemWatcherFactory(IFileSystem fileSystem)
    {
        _mock = new MockFileSystemWatcher(fileSystem);
        FileSystem = fileSystem;
    }

    public MockFileSystemWatcherFactory(IFileSystem fileSystem, MockFileSystemWatcher mock)
    {
        _mock = mock;
        FileSystem = fileSystem;
    }

    public IFileSystemWatcher CreateNew() => _mock;

    public IFileSystemWatcher CreateNew(string path) => _mock;

    public IFileSystemWatcher CreateNew(string path, string filter) => _mock;

    public IFileSystemWatcher New() => _mock;

    public IFileSystemWatcher New(string path) => _mock;

    public IFileSystemWatcher New(string path, string filter) => _mock;

    public IFileSystemWatcher? Wrap(FileSystemWatcher? fileSystemWatcher)
    {
        throw new NotImplementedException();
    }
}
    
public class MockFileSystemWatcher : IFileSystemWatcher
{
    public void Dispose()
    {
    }

    public void BeginInit()
    {
    }

    public void EndInit()
    {
    }

    public IWaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
    {
        throw new NotImplementedException();
    }

    public IWaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
    {
        throw new NotImplementedException();
    }

    public IWaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public IContainer? Container { get; set; } = null;

    public IFileSystem FileSystem { get; }

    public bool IncludeSubdirectories { get; set; }
    public bool EnableRaisingEvents { get; set; }
    public string Filter { get; set; } = string.Empty;
    public Collection<string> Filters { get; } = new();
    public int InternalBufferSize { get; set; }
    public NotifyFilters NotifyFilter { get; set; }
    public string Path { get; set; } = string.Empty;
    public ISite? Site { get; set; } = null;
    public ISynchronizeInvoke? SynchronizingObject { get; set; } = null;
    public event FileSystemEventHandler? Changed;
    public event FileSystemEventHandler? Created;
    public event FileSystemEventHandler? Deleted;
    public event ErrorEventHandler? Error;
    public event RenamedEventHandler? Renamed;

    public MockFileSystemWatcher(IFileSystem fileSystem)
    {
        FileSystem = fileSystem;
    }
    
    public void MarkCreated(FilePath path)
    {
        if (Created == null) return;
        Created(this, new FileSystemEventArgs(
            WatcherChangeTypes.Created,
            System.IO.Path.GetDirectoryName(path)!,
            System.IO.Path.GetFileName(path)));
    }

    public void MarkRenamed(FilePath from, FileName to)
    {
        if (Renamed == null) return;
        Renamed(this, new RenamedEventArgs(
            WatcherChangeTypes.Renamed,
            from.Directory!.Value.Path,
            to.String,
            from.Name.String));
    }

    public void MarkDeleted(FilePath path)
    {
        Deleted?.Invoke(this, new FileSystemEventArgs(
            WatcherChangeTypes.Deleted,
            System.IO.Path.GetDirectoryName(path)!,
            System.IO.Path.GetFileName(path)));
    }

    public void MarkChanged(FilePath path)
    {
        Changed?.Invoke(this, new FileSystemEventArgs(
            WatcherChangeTypes.Changed,
            System.IO.Path.GetDirectoryName(path)!,
            System.IO.Path.GetFileName(path)));   
    }
}