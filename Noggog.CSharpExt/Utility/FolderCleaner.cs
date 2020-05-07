using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    /*
     * Will track which files are written after its creation, and which are stale, and delete any files that have not been
     * updated.  Based off file creation date timestamps.
     */
    public class FolderCleaner : IDisposable
    {
        private readonly DateTime _startTime;
        private readonly DirectoryPath _dir;
        private readonly CleanType _clean;

        public enum CleanType
        {
            AccessTime,
            WriteTime,
            CreationTime,
        }

        public FolderCleaner(string dir, CleanType clean)
        {
            this._startTime = DateTime.Now;
            this._clean = clean;
            this._dir = new DirectoryPath(dir);
        }

        public FolderCleaner(DirectoryPath dir, CleanType clean)
        {
            this._startTime = DateTime.Now;
            this._dir = dir;
            this._clean = clean;
        }

        public void Dispose()
        {
            foreach (var item in _dir.Info.EnumerateFilesRecursive())
            {
                item.Refresh();
                DateTime cleanTime;
                switch (_clean)
                {
                    case CleanType.AccessTime:
                        cleanTime = item.LastAccessTime;
                        break;
                    case CleanType.WriteTime:
                        cleanTime = item.LastWriteTime;
                        break;
                    case CleanType.CreationTime:
                        cleanTime = item.CreationTime;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (item.LastAccessTime < _startTime)
                {
                    item.Delete();
                }
            }

            foreach (var item in _dir
                .EnumerateDirectories(recursive: true, includeSelf: true)
                .Reverse())
            {
                if (item.Empty)
                {
                    item.DeleteEntireFolder();
                }
            }
        }
    }
}
