using Noggog.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public struct FilePath : IEquatable<FilePath>, IPath
    {
        private readonly string _fullPath;
        private readonly FileInfo _fileInfo;
        private readonly string _originalPath;

        public DirectoryPath Directory => new DirectoryPath(_fileInfo.Directory.FullName);
        public string Path => _fullPath;
        public string RelativePath => _originalPath;
        public string Name => _fileInfo.Name;
        public string Extension => _fileInfo.Extension;
        public string NameWithoutExtension => _fileInfo.Name.Substring(0, _fileInfo.Name.LastIndexOf(_fileInfo.Extension));
        public bool Exists
        {
            get
            {
                _fileInfo?.Refresh();
                return _fileInfo?.Exists ?? false;
            }
        }
        public long Length
        {
            get
            {
                _fileInfo.Refresh();
                return _fileInfo.Length;
            }
        }
        public FileInfo Info => _fileInfo;

        public FilePath(string path)
        {
            this._originalPath = path;
            this._fileInfo = new FileInfo(path);
            this._fullPath = System.IO.Path.GetFullPath(path);
        }

        public bool Equals(FilePath other)
        {
            if (!this._fullPath.Equals(other._fullPath, StringComparison.OrdinalIgnoreCase)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FilePath rhs)) return false;
            return Equals(rhs);
        }

        public string GetRelativePathTo(DirectoryPath relativeTo)
        {
            return PathExt.MakeRelativePath(
                relativeTo.Path + "\\",
                this._fullPath);
        }

        public string GetRelativePathTo(FilePath relativeTo)
        {
            return PathExt.MakeRelativePath(
                relativeTo.Path,
                this._fullPath);
        }

        public void Delete()
        {
            this._fileInfo.Refresh();
            if (this._fileInfo.Exists)
            {
                this._fileInfo.Delete();
            }
        }

        public override int GetHashCode()
        {
            return this._fullPath.GetHashCode(StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return this._fileInfo.FullName;
        }

        public FileStream OpenRead()
        {
            return _fileInfo.OpenRead();
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
