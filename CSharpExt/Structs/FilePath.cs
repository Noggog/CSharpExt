using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct FilePath : IEquatable<FilePath>
    {
        private readonly StringCaseAgnostic _fullPath;
        private readonly FileInfo _fileInfo;
        private readonly string _originalPath;

        public DirectoryPath Directory => new DirectoryPath(_fileInfo.Directory.FullName);
        public string Path => _fullPath;
        public string RelativePath => _originalPath;
        public string Name => _fileInfo.Name;

        public FilePath(string path)
        {
            this._originalPath = path;
            this._fileInfo = new FileInfo(path);
            this._fullPath = StandardizePath(path);
        }

        public static StringCaseAgnostic StandardizePath(string path)
        {
            return System.IO.Path.GetFullPath(path);
        }

        public bool Equals(FilePath other)
        {
            if (!this._fullPath.Equals(other._fullPath)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FilePath rhs)) return false;
            return Equals(rhs);
        }

        public override int GetHashCode()
        {
            return this._fullPath.GetHashCode();
        }

        public override string ToString()
        {
            return this._fileInfo?.FullName;
        }

        public static implicit operator FilePath(FileInfo info)
        {
            return new FilePath(info.FullName);
        }

        public static implicit operator FilePath(string path)
        {
            return new FilePath(path);
        }
    }
}
