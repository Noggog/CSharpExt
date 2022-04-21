using System.IO.Abstractions;

namespace Noggog.Utility;

public interface IRandomTempFileProvider
{
    ITempFile Create(
        string? extraDirectoryPaths = null,
        bool deleteAfter = true, 
        bool createFolder = true, 
        string? suffix = null);
}
    
public interface ITempFileProvider
{
    ITempFile Create(
        FilePath path,
        bool deleteAfter = true,
        bool createFolder = true);
}

public class TempFileProvider : ITempFileProvider, IRandomTempFileProvider
{
    private readonly IFileSystem _fileSystem;

    public TempFileProvider(
        IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
        
    public ITempFile Create(
        FilePath path,
        bool deleteAfter = true,
        bool createFolder = true)
    {
        return new TempFile(
            path, 
            deleteAfter: deleteAfter,
            createFolder: createFolder, 
            _fileSystem);
    }
        
    public ITempFile Create(
        string? extraDirectoryPaths = null,
        bool deleteAfter = true, 
        bool createFolder = true, 
        string? suffix = null)
    {
        return new TempFile(
            extraDirectoryPaths: extraDirectoryPaths, 
            deleteAfter: deleteAfter,
            createFolder: createFolder, 
            suffix: suffix,
            fileSystem: _fileSystem);
    }
}