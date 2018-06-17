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
            using (FileStream fs2 = second.OpenRead())
            {
                byte[] one = new byte[BYTES_TO_READ];
                byte[] two = new byte[BYTES_TO_READ];

                for (int i = 0; i < iterations; i++)
                {
                    fs1.Read(one, 0, BYTES_TO_READ);
                    fs2.Read(two, 0, BYTES_TO_READ);

                    if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                        return false;
                }
            }

            return true;
        }
    }
}
