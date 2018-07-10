using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StreamExt
    {
        public static long Remaining(this Stream stream)
        {
            return stream.Length - stream.Position;
        }
    }
}
