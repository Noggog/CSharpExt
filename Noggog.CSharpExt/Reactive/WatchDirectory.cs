using System.IO.Abstractions;
using System.Reactive;

namespace Noggog.Reactive;

public interface IWatchDirectory
{
    IObservable<Unit> Watch(DirectoryPath path, bool throwIfInvalidPath = false);
}

public class WatchDirectory : IWatchDirectory
{
    private readonly IFileSystem _FileSystem;

    public WatchDirectory(IFileSystem fileSystem)
    {
        _FileSystem = fileSystem;
    }
        
    public IObservable<Unit> Watch(DirectoryPath path, bool throwIfInvalidPath = false)
    {
        return ObservableExt.WatchFolder(path, throwIfInvalidPath, _FileSystem.FileSystemWatcher);
    }
}