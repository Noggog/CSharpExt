using System;
using System.IO.Abstractions;
using System.Reactive;

namespace Noggog.Reactive
{
    public interface IWatchFile
    {
        IObservable<Unit> Watch(FilePath path, bool throwIfInvalidPath = false);
    }

    public class WatchFile : IWatchFile
    {
        private readonly IFileSystem _FileSystem;

        public WatchFile(IFileSystem fileSystem)
        {
            _FileSystem = fileSystem;
        }
        
        public IObservable<Unit> Watch(FilePath path, bool throwIfInvalidPath = false)
        {
            return ObservableExt.WatchFile(path, throwIfInvalidPath, _FileSystem.FileSystemWatcher);
        }
    }
}