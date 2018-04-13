using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    public class TempFolder : IDisposable
    {
        public DirectoryPath Dir { get; private set; }
        public bool DeleteAfter = true;

        public TempFolder(bool deleteAfter = true)
        {
            this.Dir = new DirectoryPath(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            this.Dir.Create();
            this.DeleteAfter = deleteAfter;
        }

        public TempFolder(DirectoryPath dir, bool deleteAfter = true)
        {
            this.Dir = dir;
            if (!dir.Exists)
            {
                this.Dir.Create();
            }
            this.DeleteAfter = deleteAfter;
        }

        public TempFolder(string addedFolderPath)
            : this(new DirectoryPath(Path.Combine(Path.GetTempPath(), addedFolderPath)))
        {
        }

        public void Dispose()
        {
            if (DeleteAfter)
            {
                this.Dir.DeleteEntireFolder();
            }
        }
    }
}
