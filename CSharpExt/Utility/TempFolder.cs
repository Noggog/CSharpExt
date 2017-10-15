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

        public TempFolder()
        {
            this.Dir = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
            this.Dir.Create();
        }

        public TempFolder(DirectoryInfo dir)
        {
            this.Dir = dir;
            if (!dir.Exists)
            {
                this.Dir.Create();
            }
        }

        public void Dispose()
        {
            this.Dir.DeleteEntireFolder();
        }
    }
}
