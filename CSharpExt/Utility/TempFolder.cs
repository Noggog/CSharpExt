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
        public DirectoryInfo Dir { get; private set; }
        public bool DeleteAfter = true;

        public TempFolder(bool deleteAfter = true)
        {
            this.Dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            this.Dir.Create();
            this.DeleteAfter = deleteAfter;
        }

        public TempFolder(DirectoryInfo dir, bool deleteAfter = true)
        {
            this.Dir = dir;
            if (!dir.Exists)
            {
                this.Dir.Create();
            }
            this.DeleteAfter = deleteAfter;
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
