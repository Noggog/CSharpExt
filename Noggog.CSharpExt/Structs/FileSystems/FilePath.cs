using System.Diagnostics.CodeAnalysis;
using Noggog.Extensions;
using System.IO.Abstractions;
using System.Runtime.Serialization;

namespace Noggog;

public struct FilePath : IEquatable<FilePath>, IPath
{
    private readonly string? _fullPath;
    private readonly string? _originalPath;

    [IgnoreDataMember]
    public DirectoryPath? Directory
    {
        get
        {
            var dirPath = System.IO.Path.GetDirectoryName(_fullPath);
            if (dirPath.IsNullOrWhitespace()) return null;
            return new DirectoryPath(dirPath);
        }
    }

    [IgnoreDataMember]
    public string Path => _fullPath ?? string.Empty;

    public string RelativePath => _originalPath ?? string.Empty;

    [IgnoreDataMember]
    public FileName Name => System.IO.Path.GetFileName(Path);

    [IgnoreDataMember]
    public string Extension => System.IO.Path.GetExtension(Path);

    [IgnoreDataMember]
    public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Path);

    [IgnoreDataMember]
    public bool Exists => CheckExists();

    private FilePath(string? fullPath, string? originalPath)
    {
        _fullPath = fullPath;
        _originalPath = originalPath;
    }

    public FilePath(string relativePath)
    {
        relativePath = IFileSystemExt.CleanDirectorySeparators(relativePath);
        _originalPath = relativePath;
        _fullPath = relativePath == string.Empty ? string.Empty : System.IO.Path.GetFullPath(relativePath);
    }

    public bool CheckExists(IFileSystem? fs = null) => fs.GetOrDefault().File.Exists(_fullPath);

    public static bool operator ==(FilePath lhs, FilePath rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(FilePath lhs, FilePath rhs)
    {
        return !lhs.Equals(rhs);
    }

    public bool Equals(FilePath other)
    {
        return string.Equals(Path, other.Path, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not FilePath rhs) return false;
        return Equals(rhs);
    }

    public string GetRelativePathTo(DirectoryPath relativeTo)
    {
        return PathExt.MakeRelativePath(
            relativeTo.Path + System.IO.Path.DirectorySeparatorChar,
            Path);
    }

    public string GetRelativePathTo(FilePath relativeTo)
    {
        return PathExt.MakeRelativePath(
            relativeTo.Path,
            Path);
    }

    public bool IsUnderneath(DirectoryPath dir)
    {
        return Path.StartsWith(dir.Path, StringComparison.OrdinalIgnoreCase);
    }

    public void Delete(IFileSystem? fileSystem = null)
    {
        fileSystem = fileSystem.GetOrDefault();
        if (fileSystem.File.Exists(Path))
        {
            fileSystem.File.Delete(Path);
        }
    }

    public override int GetHashCode()
    {
#if NETSTANDARD2_0 
        return Path.ToLower().GetHashCode();
#else 
        return Path.GetHashCode(StringComparison.OrdinalIgnoreCase);
#endif
    }

    public override string ToString() => Path;

    public Stream OpenRead(IFileSystem? fileSystem = null)
    {
        return fileSystem.GetOrDefault().FileStream.Create(Path, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public FilePath MakeAbsolute()
    {
        return new FilePath(_fullPath, _fullPath);
    }

    public static implicit operator FilePath(FileInfo info)
    {
        return new FilePath(info.FullName);
    }

    public static implicit operator FilePath(string path)
    {
        return new FilePath(path);
    }

    [return: NotNullIfNotNull("path")]
    public static implicit operator FilePath?(string? path)
    {
        if (path == null) return null;
        return new FilePath(path);
    }

    public static implicit operator string(FilePath path)
    {
        return path._originalPath ?? string.Empty;
    }

    [return: NotNullIfNotNull("path")]
    public static implicit operator string?(FilePath? path)
    {
        if (path == null) return null;
        return (string)path.Value;
    }
}