using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public interface IBinaryReadStream : IDisposable
    {
        long Position { get; set; }
        long Length { get; }
        long Remaining { get; }
        bool Complete { get; }
        int Read(byte[] buffer, int offset, int amount);
        int Get(byte[] buffer, int targetOffset, int amount);
        int Read(byte[] buffer);
        int Get(byte[] buffer, int targetOffset);
        byte[] ReadBytes(int amount);
        byte[] GetBytes(int amount);
        bool ReadBool();
        bool GetBool(int offset = 0);
        byte ReadUInt8();
        byte GetUInt8(int offset = 0);
        ushort ReadUInt16();
        ushort GetUInt16(int offset = 0);
        uint ReadUInt32();
        uint GetUInt32(int offset = 0);
        ulong ReadUInt64();
        ulong GetUInt64(int offset = 0);
        sbyte ReadInt8();
        sbyte GetInt8(int offset = 0);
        short ReadInt16();
        short GetInt16(int offset = 0);
        int ReadInt32();
        int GetInt32(int offset = 0);
        long ReadInt64();
        long GetInt64(int offset = 0);
        float ReadFloat();
        float GetFloat(int offset = 0);
        double ReadDouble();
        double GetDouble(int offset = 0);
        string ReadString(int amount);
        string GetString(int amount, int offset = 0);
        void WriteTo(Stream stream, int amount);
    }
}
