using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Abstractions;

namespace Noggog.Testing.FileSystem;

public class MockFileSystemWatcherFactory : IFileSystemWatcherFactory
{
    private readonly MockFileSystemWatcher _mock;

    public MockFileSystemWatcherFactory()
    {
        _mock = new MockFileSystemWatcher();
    }

    public MockFileSystemWatcherFactory(MockFileSystemWatcher mock)
    {
        _mock = mock;
    }

    public IFileSystemWatcher CreateNew() => _mock;

    public IFileSystemWatcher CreateNew(string path) => _mock;

    public IFileSystemWatcher CreateNew(string path, string filter) => _mock;
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

    public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType)
    {
        throw new NotImplementedException();
    }

    public WaitForChangedResult WaitForChanged(WatcherChangeTypes changeType, int timeout)
    {
        throw new NotImplementedException();
    }

    public bool IncludeSubdirectories { get; set; }
    public bool EnableRaisingEvents { get; set; }
    public string Filter { get; set; } = string.Empty;
    public Collection<string> Filters { get; } = new();
    public int InternalBufferSize { get; set; }
    public NotifyFilters NotifyFilter { get; set; }
    public string Path { get; set; } = string.Empty;
    public ISite Site { get; set; } = null!;
    public ISynchronizeInvoke SynchronizingObject { get; set; } = null!;
    public event FileSystemEventHandler? Changed;
    public event FileSystemEventHandler? Created;
    public event FileSystemEventHandler? Deleted;
    public event ErrorEventHandler? Error;
    public event RenamedEventHandler? Renamed;

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