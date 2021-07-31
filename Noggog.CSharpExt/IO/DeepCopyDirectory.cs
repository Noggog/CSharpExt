using System.IO.Abstractions;

namespace Noggog.IO
{
    public interface IDeepCopyDirectory
    {
        void DeepCopy(DirectoryPath dir, DirectoryPath rhs);
    }

    public class DeepCopyDirectory : IDeepCopyDirectory
    {
        private readonly IFileSystem _fileSystem;

        public DeepCopyDirectory(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
        
        public void DeepCopy(DirectoryPath dir, DirectoryPath rhs)
        {
            _fileSystem.Directory.DeepCopy(dir, rhs);
        }
    }
}