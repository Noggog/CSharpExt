using Noggog.Extensions;
using System;
using System.IO;

namespace Noggog
{
    public struct FilePath : IEquatable<FilePath>, IPath
    {
        private readonly string? _fullPath;
        private readonly string? _originalPath;

        public DirectoryPath? Directory
        {
            get
            {
                var dirPath = System.IO.Path.GetDirectoryName(_originalPath);
                if (dirPath.IsNullOrWhitespace()) return null;
                return new DirectoryPath(dirPath);
            }
        }

        public string Path => _fullPath ?? string.Empty;
        public string RelativePath => _originalPath ?? string.Empty;
        public FileName Name => System.IO.Path.GetFileName(Path);
        public string Extension => System.IO.Path.GetExtension(Path);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Path);
        public bool Exists => System.IO.File.Exists(_fullPath);

        public FilePath(string path)
        {
            this._originalPath = path;
            this._fullPath = path == string.Empty ? string.Empty : System.IO.Path.GetFullPath(path);
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
                relativeTo.Path + "\\",
                Path);
        }

        public string GetRelativePathTo(FilePath relativeTo)
        {
            return PathExt.MakeRelativePath(
                relativeTo.Path,
                Path);
        }

        public void Delete()
        {
            if (File.Exists(Path))
            {
                File.Delete(Path);
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

        public FileStream OpenRead()
        {
            return File.OpenRead(Path);
        }

        public static implicit operator FilePath(FileInfo info)
        {
            return new FilePath(info.FullName);
        }

        public static implicit operator FilePath(string path)
        {
            return new FilePath(path);
        }

        public static implicit operator string(FilePath path)
        {
            return path._originalPath ?? string.Empty;
        }
    }
}
