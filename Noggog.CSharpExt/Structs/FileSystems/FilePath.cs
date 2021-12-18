using Noggog.Extensions;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.Serialization;

namespace Noggog
{
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

        public FilePath(string path)
        {
            this._originalPath = path.Replace('/', '\\');
            this._fullPath = path == string.Empty ? string.Empty : System.IO.Path.GetFullPath(path);
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
                relativeTo.Path + "\\",
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
