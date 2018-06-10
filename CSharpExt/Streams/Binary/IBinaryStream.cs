using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface IBinaryStream : IDisposable
    {
        long Position { get; set; }
        long Length { get; }
        long Remaining { get; }
        bool Complete { get; }
        int Read(byte[] buffer, int offset, int amount);
        int Read(byte[] buffer);
        byte[] ReadBytes(int amount);
        bool ReadBool();
        byte ReadUInt8();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();
        sbyte ReadInt8();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        float ReadFloat();
        double ReadDouble();
        string ReadString(int amount);
    }
}
