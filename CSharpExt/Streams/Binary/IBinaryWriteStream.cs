using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface IBinaryWriteStream : IDisposable
    {
        long Position { get; set; }
        long Length { get; }
        void Write(byte[] buffer, int offset, int amount);
        void Write(byte[] buffer);
        void Write(bool value);
        void Write(byte value);
        void Write(ushort value);
        void Write(uint value);
        void Write(ulong value);
        void Write(sbyte value);
        void Write(short value);
        void Write(int value);
        void Write(long value);
        void Write(float value);
        void Write(double value);
        void Write(string value);
    }
}
