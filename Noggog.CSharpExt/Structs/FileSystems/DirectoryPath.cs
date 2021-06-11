using Noggog.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
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
        public bool Exists => System.IO.Directory.Exists(Path);
        public string Path => _fullPath ?? string.Empty;
        public string RelativePath => _originalPath ?? string.Empty;
        public FileName Name => System.IO.Path.GetFileName(Path);
        public bool Empty => !System.IO.Directory.EnumerateFiles(Path).Any()
            && !System.IO.Directory.EnumerateDirectories(Path).Any();

        public DirectoryPath(string path)
        {
            path = System.IO.Path.TrimEndingDirectorySeparator(path);
            this._originalPath = path;
            this._fullPath = path == string.Empty ? string.Empty : System.IO.Path.GetFullPath(path);
        }

        public DirectoryPath(string path, string referencePath)
        {
            if (path.StartsWith(".."))
            {
                path = System.IO.Path.Combine(referencePath, path);
            }
            this._originalPath = path;
            this._fullPath = path == string.Empty ? string.Empty : System.IO.Path.GetFullPath(path);
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
            return Path.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString() => Path;

        public FilePath GetFile(string filePath)
        {
            return new FilePath(System.IO.Path.Combine(this.Path, filePath));
        }

        public bool IsSubfolderOf(DirectoryPath potentialParent)
        {
            var parent = System.IO.Directory.GetParent(Path);
            while (parent != null)
            {
                if (parent.FullName.Equals(potentialParent.Name))
                {
                    return true;
                }
                parent = System.IO.Directory.GetParent(parent.FullName);
            }
            return false;
        }

        public bool TryDeleteEntireFolder(bool disableReadOnly = true, bool deleteFolderItself = true)
        {
            try
            {
                DeleteEntireFolder(disableReadOnly, deleteFolderItself);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void DeleteEntireFolder(bool disableReadOnly = true, bool deleteFolderItself = true)
        {
            if (!Exists) return;
            var dirInfo = new DirectoryInfo(Path);
            dirInfo.DeleteEntireFolder(disableReadOnly, deleteFolderItself);
        }

        public void DeleteContainedFiles(bool recursive)
        {
            if (!Exists) return;
            var dirInfo = new DirectoryInfo(Path);
            dirInfo.DeleteContainedFiles(recursive);
        }

        public void Create()
        {
            System.IO.Directory.CreateDirectory(Path);
        }

        public string GetRelativePathTo(DirectoryPath relativeTo)
        {
            return PathExt.MakeRelativePath(
                relativeTo.Path + "\\",
                Path + "\\");
        }

        public IEnumerable<FilePath> EnumerateFiles(bool recursive = false, string? searchPattern = null)
        {
            var ret = (searchPattern == null ? System.IO.Directory.EnumerateFiles(Path) : System.IO.Directory.EnumerateFiles(Path, searchPattern))
                .Select(x => new FilePath(x));
            if (!recursive) return ret;
            return ret.Concat(EnumerateDirectories(includeSelf: false, recursive: true)
                .SelectMany(d => d.EnumerateFiles(recursive: false, searchPattern: searchPattern)));
        }

        public IEnumerable<DirectoryPath> EnumerateDirectories(bool includeSelf, bool recursive)
        {
            if (!Exists) return EnumerableExt<DirectoryPath>.Empty;
            var ret = System.IO.Directory.EnumerateDirectories(Path)
                .Select(x => new DirectoryPath(x));
            if (recursive)
            {
                ret = ret
                    .Concat(System.IO.Directory.EnumerateDirectories(Path)
                    .Select(x => new DirectoryPath(x))
                    .SelectMany(d => d.EnumerateDirectories(includeSelf: false, recursive: true)));
            }
            if (includeSelf)
            {
                ret = this.AsEnumerable().Concat(ret);
            }
            return ret;
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
            return dir.Path;
        }

        public static implicit operator ReadOnlySpan<char>(DirectoryPath dir)
        {
            return dir.Path;
        }
    }
}
