using System.IO.Abstractions;

namespace Noggog.Utility;

public interface ITempFile : IDisposable
{
    FilePath File { get; set; }
}

public class TempFile : ITempFile
{
    private readonly IFileSystem _fileSystem;
        
    public FilePath File { get; set; }
    public bool DeleteAfter = true;

    public TempFile(
        string? extraDirectoryPaths = null,
        bool deleteAfter = true, 
        bool createFolder = true, 
        string? suffix = null,
        IFileSystem? fileSystem = null)
    {
        _fileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
            
        var path = $"{Path.GetRandomFileName()}{suffix}";
        if (extraDirectoryPaths != null)
        {
            path = Path.Combine(extraDirectoryPaths, path);
        }
        File = new FilePath(Path.Combine(Path.GetTempPath(), path));
        DeleteAfter = deleteAfter;
            
        if (createFolder 
            && File.Directory != null 
            && !_fileSystem.Directory.Exists(File.Directory.Value))
        {
            _fileSystem.Directory.CreateDirectory(File.Directory.Value);
        }
    }

    public TempFile(
        FilePath file, 
        bool deleteAfter = true, 
        bool createFolder = true,
        IFileSystem? fileSystem = null)
    {
        _fileSystem = fileSystem ?? IFileSystemExt.DefaultFilesystem;
        File = file;
        DeleteAfter = deleteAfter;
            
        if (createFolder 
            && file.Directory != null 
            && !_fileSystem.Directory.Exists(file.Directory.Value))
        {
            file.Directory.Value.Create();
        }
    }

    public void Dispose()
    {
        if (DeleteAfter)
        {
            _fileSystem.File.Delete(File);
        }
    }
}