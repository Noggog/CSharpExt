using System.IO;
using System.IO.Abstractions;

namespace Noggog.Utility
{
    public interface IRandomTempFolderProvider
    {
        ITempFolder Create(bool deleteAfter = true, bool throwIfUnsuccessfulDisposal = true);
    }
    
    public interface ITempFolderProvider
    {
        ITempFolder Create(
            string path,
            bool deleteAfter = true, 
            bool throwIfUnsuccessfulDisposal = true);

        ITempFolder CreateByAddedPath(string addedFolderPath, bool deleteAfter = true, bool throwIfUnsuccessfulDisposal = true);
    }

    public class TempFolderProvider : ITempFolderProvider, IRandomTempFolderProvider
    {
        private readonly IFileSystem _fileSystem;

        public TempFolderProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public ITempFolder Create(bool deleteAfter = true, bool throwIfUnsuccessfulDisposal = true)
        {
            return TempFolder.Factory(
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: _fileSystem);
        }

        public ITempFolder Create(
            string path,
            bool deleteAfter = true, 
            bool throwIfUnsuccessfulDisposal = true)
        {
            return TempFolder.FactoryByPath(
                new DirectoryPath(path),
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: _fileSystem);
        }

        public ITempFolder CreateByAddedPath(string addedFolderPath, bool deleteAfter = true, bool throwIfUnsuccessfulDisposal = true)
        {
            return TempFolder.FactoryByAddedPath(
                new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)),
                deleteAfter: deleteAfter,
                throwIfUnsuccessfulDisposal: throwIfUnsuccessfulDisposal,
                fileSystem: _fileSystem);
        }
    }
}