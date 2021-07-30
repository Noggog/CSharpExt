using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Noggog.Utility
{
    public static class FileComparison
    {
        // https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net
        const int BYTES_TO_READ = sizeof(Int64);
        public static bool FilesAreEqual(FilePath first, FilePath second)
        {
            var firstInfo = new FileInfo(first.Path);
            var secondInfo = new FileInfo(second.Path);
            if (firstInfo.Length != secondInfo.Length)
                return false;

            if (string.Equals(first.Path, second.Path, StringComparison.OrdinalIgnoreCase))
                return true;

            int iterations = (int)Math.Ceiling((double)firstInfo.Length / BYTES_TO_READ);

            using (var fs1 = first.OpenRead())
            {
                using (var fs2 = second.OpenRead())
                {
                    return fs1.ContentsEqual(fs2);
                }
            }
        }

        private static readonly FileCompare comp = new FileCompare();
        public static bool FoldersAreEqual(DirectoryPath first, DirectoryPath second)
        {
            return first.EnumerateFiles(recursive: true)
                .SequenceEqual(
                    second.EnumerateFiles(recursive: true),
                    comp);
        }

        class FileCompare : System.Collections.Generic.IEqualityComparer<FilePath>
        {
            public bool Equals(FilePath x, FilePath y)
            {
                if (!string.Equals(x.Name, y.Name)) return false;
                return FileComparison.FilesAreEqual(x, y);
            }

            public int GetHashCode(FilePath fi)
            {
                throw new NotImplementedException();
            }
        }
    }
}
