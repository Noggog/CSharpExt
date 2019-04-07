using Noggog.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct DirectoryPath : IEquatable<DirectoryPath>
    {
        private readonly StringCaseAgnostic _fullPath;
        private readonly FileInfo _fileInfo;
        private readonly DirectoryInfo _dirInfo;
        public DirectoryPath Directory => new DirectoryPath(_fileInfo.Directory.FullName);
        public bool Exists => _dirInfo?.Exists() ?? false;
        public string Path => _fullPath;
        public string Name => _dirInfo.Name;
        public bool Empty => !_dirInfo.EnumerateFiles().Any()
            && !_dirInfo.EnumerateDirectories().Any();
        public DirectoryInfo Info => _dirInfo;

        public DirectoryPath(string path)
        {
            this._fileInfo = new FileInfo(path);
            this._fullPath = FilePath.StandardizePath(path);
            this._dirInfo = new DirectoryInfo(this._fullPath);
        }

        public DirectoryPath(string path, string referencePath)
        {
            if (path.StartsWith(".."))
            {
                path = System.IO.Path.Combine(referencePath, path);
            }
            this._fileInfo = new FileInfo(path);
            this._fullPath = FilePath.StandardizePath(path);
            this._dirInfo = new DirectoryInfo(this._fullPath);
        }

        public bool Equals(DirectoryPath other)
        {
            if (!this._fullPath.Equals(other._fullPath)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DirectoryPath rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this._fullPath.GetHashCode();
        }

        public override string ToString()
        {
            return this._dirInfo?.FullName;
        }

        public FilePath GetFile(string filePath)
        {
            return new FilePath(System.IO.Path.Combine(this.Path, filePath));
        }

        public bool IsSubfolderOf(DirectoryPath rhs)
        {
            return this._dirInfo.IsSubfolderOf(rhs._dirInfo);
        }

        public bool DeleteEntireFolder(bool disableReadOnly = true, bool deleteFolderItself = true)
        {
            return this._dirInfo.DeleteEntireFolder(disableReadOnly, deleteFolderItself: deleteFolderItself);
        }

        public void DeleteContainedFiles(bool recursive)
        {
            this._dirInfo.DeleteContainedFiles(recursive);
        }

        public void Create()
        {
            if (this.Exists) return;
            this._dirInfo.Create();
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
                return this._dirInfo.EnumerateFilesRecursive();
            }
            else
            {
                this._dirInfo.Refresh();
                return this._dirInfo.EnumerateFiles();
            }
        }

        public IEnumerable<FilePath> EnumerateFiles(bool recursive = false)
        {
            foreach (var file in (recursive ? this._dirInfo.EnumerateFilesRecursive() : this._dirInfo.EnumerateFiles()))
            {
                yield return new FilePath(file.FullName);
            }
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectoryInfos(bool includeSelf, bool recursive)
        {
            return this._dirInfo.EnumerateDirectories(
                includeSelf: includeSelf,
                recursive: recursive);
        }

        public IEnumerable<DirectoryPath> EnumerateDirectories(bool includeSelf, bool recursive)
        {
            foreach (var file in this._dirInfo.EnumerateDirectories(
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
