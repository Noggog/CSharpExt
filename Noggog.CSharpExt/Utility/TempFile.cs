using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    public class TempFile : IDisposable
    {
        public FilePath File { get; set; }
        public bool DeleteAfter = true;

        public TempFile(string? extraDirectoryPaths = null, bool deleteAfter = true, bool createFolder = true, string? suffix = null)
        {
            var path = $"{Path.GetRandomFileName()}{suffix}";
            if (extraDirectoryPaths != null)
            {
                path = Path.Combine(extraDirectoryPaths, path);
            }
            File = new FilePath(Path.Combine(Path.GetTempPath(), path));
            if (createFolder && !File.Directory.Exists)
            {
                File.Directory.Create();
            }
            this.DeleteAfter = deleteAfter;
        }

        public TempFile(FilePath file, bool deleteAfter = true, bool createFolder = true)
        {
            this.File = file;
            if (createFolder && !file.Directory.Exists)
            {
                file.Directory.Create();
            }
            this.DeleteAfter = deleteAfter;
        }

        public void Dispose()
        {
            if (DeleteAfter)
            {
                this.File.Delete();
            }
        }
    }
}
