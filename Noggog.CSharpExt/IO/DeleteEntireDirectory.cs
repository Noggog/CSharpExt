using System.IO.Abstractions;

namespace Noggog.IO;

public interface IDeleteEntireDirectory
{
    void DeleteEntireFolder(
        DirectoryPath dir,
        bool disableReadOnly = true,
        bool deleteFolderItself = true);
}

public class DeleteEntireDirectory : IDeleteEntireDirectory
{
    private readonly IFileSystem _fileSystem;

    public DeleteEntireDirectory(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
        
    public void DeleteEntireFolder(
        DirectoryPath dir,
        bool disableReadOnly = true,
        bool deleteFolderItself = true)
    {
        dir.DeleteEntireFolder(
            disableReadOnly: disableReadOnly,
            deleteFolderItself: deleteFolderItself,
            fileSystem: _fileSystem);
    }
}