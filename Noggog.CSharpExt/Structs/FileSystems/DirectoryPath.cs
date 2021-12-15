using Noggog.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Noggog
{
    public struct DirectoryPath : IEquatable<DirectoryPath>, IPath
    {
        private readonly string? _fullPath;
        private readonly string? _originalPath;
        public DirectoryPath? Directory
        {
            get
            {
                var dirPath = System.IO.Path.GetDirectoryName(_fullPath);
                if (dirPath.IsNullOrWhitespace()) return null;
                return new DirectoryPath(dirPath);
            }
        }

        public bool Exists => CheckExists();
        public bool CheckExists(IFileSystem? fs = null) => fs.GetOrDefault().Directory.Exists(Path);
        public string Path => _fullPath ?? string.Empty;
        public string RelativePath => _originalPath ?? string.Empty;
        public FileName Name => System.IO.Path.GetFileName(Path);
        public bool Empty => CheckEmpty();
        
        public DirectoryPath(string path)
        {
#if NETSTANDARD2_0
            path = path.TrimEnd('/').TrimEnd('\\');
#else 
            path = System.IO.Path.TrimEndingDirectorySeparator(path);
#endif
            this._originalPath = path.Replace('/', '\\');;
            this._fullPath = path == string.Empty ? string.Empty : System.IO.Path.GetFullPath(path);
        }

        public static bool operator ==(DirectoryPath lhs, DirectoryPath rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DirectoryPath lhs, DirectoryPath rhs)
        {
            return !lhs.Equals(rhs);
        }

        public bool Equals(DirectoryPath other)
        {
            return Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not DirectoryPath rhs) return false;
            return Equals(rhs);
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

        public FilePath GetFile(string filePath)
        {
            return new FilePath(System.IO.Path.Combine(this.Path, filePath));
        }

        [Obsolete("Use IsUnderneath instead")]
        public bool IsSubfolderOf(DirectoryPath potentialParent) => IsUnderneath(potentialParent);
        
        // ToDo
        // Can maybe be improved to not check full path on each level
        public bool IsUnderneath(DirectoryPath potentialParent)
        {
            var parent = System.IO.Directory.GetParent(Path);
            while (parent != null)
            {
                if (parent.FullName.Equals(potentialParent.Path, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
                parent = System.IO.Directory.GetParent(parent.FullName);
            }
            return false;
        }

        public bool TryDeleteEntireFolder(
            bool disableReadOnly = true,
            bool deleteFolderItself = true,
            IFileSystem? fileSystem = null)
        {
            try
            {
                DeleteEntireFolder(disableReadOnly, deleteFolderItself, fileSystem);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DeleteEntireFolder(
            bool disableReadOnly = true,
            bool deleteFolderItself = true,
            IFileSystem? fileSystem = null)
        {
            fileSystem.GetOrDefault().Directory
                .DeleteEntireFolder(
                    Path,
                    disableReadOnly: disableReadOnly,
                    deleteFolderItself: deleteFolderItself);
        }

        public void Create(IFileSystem? fileSystem = null)
        {
            fileSystem.GetOrDefault().Directory.CreateDirectory(Path);
        }

        public void Delete(IFileSystem? fileSystem = null)
        {
            fileSystem.GetOrDefault().Directory.Delete(Path);
        }

        public bool CheckEmpty(IFileSystem? fs = null)
        {
            if (!Exists) return true;
            fs = fs.GetOrDefault();
            return !fs.Directory.EnumerateFiles(Path).Any()
                && !fs.Directory.EnumerateDirectories(Path).Any();
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

        public IEnumerable<FilePath> EnumerateFiles(
            bool recursive = false, 
            string? searchPattern = null,
            IFileSystem? fileSystem = null)
        {
            return fileSystem.GetOrDefault().Directory.EnumerateFilePaths(
                Path,
                searchPattern: searchPattern,
                recursive: recursive);
        }

        public IEnumerable<DirectoryPath> EnumerateDirectories(
            bool includeSelf, 
            bool recursive,
            IFileSystem? fileSystem = null)
        {
            return fileSystem.GetOrDefault().Directory.EnumerateDirectoryPaths(
                Path,
                includeSelf: includeSelf,
                recursive: recursive);
        }

        public static implicit operator DirectoryPath(DirectoryInfo info)
        {
            return new DirectoryPath(info.FullName);
        }

        public static implicit operator DirectoryPath(string path)
        {
            return new DirectoryPath(path);
        }

        public static implicit operator string(DirectoryPath dir)
        {
            return dir.RelativePath;
        }
        
#if NETSTANDARD2_0 
#else 
        public static implicit operator ReadOnlySpan<char>(DirectoryPath dir)
        {
            return dir.RelativePath;
        }
#endif
    }
}
