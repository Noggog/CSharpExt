using System.IO.Abstractions;

namespace Noggog.IO;

public interface IExportStringToFile
{
    void ExportToFile(FilePath file, string str, bool onlyIfChanged = true, IFileSystem? fileSystem = null);
}

public class ExportStringToFile : IExportStringToFile
{
    public void ExportToFile(FilePath file, string str, bool onlyIfChanged = true, IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        if (onlyIfChanged && fileSystem.File.Exists(file))
        {
            var existStr = fileSystem.File.ReadAllText(file.Path);
            if (str.Equals(existStr)) return;
        }

        fileSystem.Directory.CreateDirectory(file.Directory);
        fileSystem.File.WriteAllText(file.Path, str);
    }
}