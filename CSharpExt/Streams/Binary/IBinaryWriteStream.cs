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
        void WriteBool(bool value);
        void WriteUInt8(byte value);
        void WriteUInt16(ushort value);
        void WriteUInt32(uint value);
        void WriteUInt64(ulong value);
        void WriteInt8(sbyte value);
        void WriteInt16(short value);
        void WriteInt32(int value);
        void WriteInt64(long value);
        void WriteFloat(float value);
        void WriteDouble(double value);
        void WriteString(string value);
    }
}
