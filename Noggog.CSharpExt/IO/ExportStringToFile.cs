using System.IO.Abstractions;

namespace Noggog.IO;

public interface IExportStringToFile
{
    void ExportToFile(FilePath file, string str, bool onlyIfChanged = true);
}

public class ExportStringToFile : IExportStringToFile
{
    private readonly IFileSystem _fileSystem;

    public ExportStringToFile(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    
    public void ExportToFile(FilePath file, string str, bool onlyIfChanged = true)
    {
        if (onlyIfChanged && _fileSystem.File.Exists(file))
        {
            var existStr = _fileSystem.File.ReadAllText(file.Path);
            if (str.Equals(existStr)) return;
        }

        if (file.Directory != null)
        {
            _fileSystem.Directory.CreateDirectory(file.Directory);
        }
        
        _fileSystem.File.WriteAllText(file.Path, str);
    }
}