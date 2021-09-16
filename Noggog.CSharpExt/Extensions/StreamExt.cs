using System;
using System.IO;

namespace Noggog
{
    public static class StreamExt
    {
        public static long Remaining(this Stream stream)
        {
            return stream.Length - stream.Position;
        }

        // https://stackoverflow.com/questions/1358510/how-to-compare-2-files-fast-using-net
        const int BYTES_TO_READ = sizeof(Int64);
        public static bool ContentsEqual(this Stream first, Stream second)
        {
            if (first.Length != second.Length)
                return false;

            int iterations = (int)Math.Ceiling((double)first.Length / BYTES_TO_READ);

            byte[] one = new byte[BYTES_TO_READ];
            byte[] two = new byte[BYTES_TO_READ];

            for (int i = 0; i < iterations; i++)
            {
                if (0 == first.Read(one, 0, BYTES_TO_READ)) return false;
                if (0 == second.Read(two, 0, BYTES_TO_READ)) return false;

                if (BitConverter.ToInt64(one, 0) != BitConverter.ToInt64(two, 0))
                    return false;
            }

            return true;
        }
    }
}
