using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace Noggog.Testing.FileSystem
{
    public class NoggogMockFileSystem : MockFileSystem
    {
        private IFileSystemWatcherFactory? _fileSystemWatcher;

        public override IFileSystemWatcherFactory FileSystemWatcher
        {
            get
            {
                if (_fileSystemWatcher == null) return base.FileSystemWatcher;
                return _fileSystemWatcher;
            }
        }
        
        public NoggogMockFileSystem(IFileSystemWatcherFactory? fileSystemWatcher = null)
        {
            _fileSystemWatcher = fileSystemWatcher;
        }

        public NoggogMockFileSystem(IDictionary<string, MockFileData> files, string currentDirectory = "", IFileSystemWatcherFactory? fileSystemWatcher = null)
            : base(files, currentDirectory)
        {
            _fileSystemWatcher = fileSystemWatcher;
        }
    }
}