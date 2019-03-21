using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Utility
{
    public static class FileComparison
    {
        // https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net
        const int BYTES_TO_READ = sizeof(Int64);
        public static bool FilesAreEqual(FilePath first, FilePath second)
        {
            if (first.Length != second.Length)
                return false;

            if (string.Equals(first.Path, second.Path, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            using (FileStream fs1 = first.OpenRead())
            {
                using (FileStream fs2 = second.OpenRead())
                {
                    return fs1.ContentsEqual(fs2);
                }
            }
        }

        private static readonly FileCompare comp = new FileCompare();
        public static bool FoldersAreEqual(DirectoryPath first, DirectoryPath second)
        {
            return first.Info.GetFiles("*.*", System.IO.SearchOption.AllDirectories)
                .SequenceEqual(
                    second.Info.GetFiles("*.*", System.IO.SearchOption.AllDirectories),
                    comp);
        }

        class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (f1.Name != f2.Name) return false;
                return FileComparison.FilesAreEqual(f1, f2);
            }
            
            public int GetHashCode(System.IO.FileInfo fi)
            {
                throw new NotImplementedException();
            }
        }
    }
}
