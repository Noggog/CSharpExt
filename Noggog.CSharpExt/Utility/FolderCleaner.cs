using System;
using System.IO;
using System.Linq;

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
            foreach (var item in _dir.EnumerateFiles(recursive: true))
            {
                var info = new FileInfo(item.Path);
                var cleanTime = _clean switch
                {
                    CleanType.AccessTime => info.LastAccessTime,
                    CleanType.WriteTime => info.LastWriteTime,
                    CleanType.CreationTime => info.CreationTime,
                    _ => throw new NotImplementedException(),
                };
                if (info.LastAccessTime < _startTime)
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
