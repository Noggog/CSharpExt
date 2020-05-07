using Noggog.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct DirectoryPath : IEquatable<DirectoryPath>, IPath
    {
        private readonly string _fullPath;
        private readonly FileInfo _fileInfo;
        public DirectoryInfo Info { get; }
        public DirectoryPath Directory => new DirectoryPath(_fileInfo.Directory.FullName);
        public bool Exists => Info?.Exists() ?? false;
        public string Path => _fullPath;
        public string Name => Info.Name;
        public bool Empty => !Info.EnumerateFiles().Any()
            && !Info.EnumerateDirectories().Any();

        public DirectoryPath(string path)
        {
            this._fileInfo = new FileInfo(path);
            this._fullPath = System.IO.Path.GetFullPath(path);
            this.Info = new DirectoryInfo(this._fullPath);
        }

        public DirectoryPath(string path, string referencePath)
        {
            if (path.StartsWith(".."))
            {
                path = System.IO.Path.Combine(referencePath, path);
            }
            this._fileInfo = new FileInfo(path);
            this._fullPath = System.IO.Path.GetFullPath(path);
            this.Info = new DirectoryInfo(this._fullPath);
        }

        public bool Equals(DirectoryPath other)
        {
            if (!this._fullPath.Equals(other._fullPath, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DirectoryPath rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this._fullPath.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return this.Info.FullName;
        }

        public FilePath GetFile(string filePath)
        {
            return new FilePath(System.IO.Path.Combine(this.Path, filePath));
        }

        public bool IsSubfolderOf(DirectoryPath rhs)
        {
            return this.Info.IsSubfolderOf(rhs.Info);
        }

        public bool DeleteEntireFolder(bool disableReadOnly = true, bool deleteFolderItself = true)
        {
            return this.Info.DeleteEntireFolder(disableReadOnly, deleteFolderItself: deleteFolderItself);
        }

        public void DeleteContainedFiles(bool recursive)
        {
            this.Info.DeleteContainedFiles(recursive);
        }

        public void Create()
        {
            if (this.Exists) return;
            this.Info.Create();
        }

        public string GetRelativePathTo(DirectoryPath relativeTo)
        {
            return PathExt.MakeRelativePath(
                relativeTo.Path + "\\",
                this._fullPath + "\\");
        }

        public IEnumerable<FileInfo> EnumerateFileInfos(bool recursive = false)
        {
            if (recursive)
            {
                return this.Info.EnumerateFilesRecursive();
            }
            else
            {
                this.Info.Refresh();
                return this.Info.EnumerateFiles();
            }
        }

        public IEnumerable<FilePath> EnumerateFiles(bool recursive = false)
        {
            foreach (var file in (recursive ? this.Info.EnumerateFilesRecursive() : this.Info.EnumerateFiles()))
            {
                yield return new FilePath(file.FullName);
            }
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectoryInfos(bool includeSelf, bool recursive)
        {
            return this.Info.EnumerateDirectories(
                includeSelf: includeSelf,
                recursive: recursive);
        }

        public IEnumerable<DirectoryPath> EnumerateDirectories(bool includeSelf, bool recursive)
        {
            foreach (var file in this.Info.EnumerateDirectories(
                includeSelf: includeSelf, 
                recursive: recursive))
            {
                yield return new DirectoryPath(file.FullName);
            }
        }

        public static implicit operator DirectoryPath(DirectoryInfo info)
        {
            return new DirectoryPath(info.FullName);
        }

        public static implicit operator DirectoryPath(string path)
        {
            return new DirectoryPath(path);
        }
    }
}
