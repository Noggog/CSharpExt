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
        public bool Exists => _dirInfo.Exists();
        public string Path => _fullPath;
        public string Name => _dirInfo.Name;

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

        public bool IsSubfolderOf(DirectoryPath rhs)
        {
            return this._dirInfo.IsSubfolderOf(rhs._dirInfo);
        }

        public bool DeleteEntireFolder(bool disableReadOnly = true)
        {
            return this._dirInfo.DeleteEntireFolder(disableReadOnly);
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
