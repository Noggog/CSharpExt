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

        /// <summary>
        /// True if the backing data is reliable between reading events
        /// </summary>
        bool IsPersistantBacking { get; }
        ReadOnlySpan<byte> RemainingSpan { get; }
        ReadOnlyMemorySlice<byte> RemainingMemory { get; }
        int Read(byte[] buffer, int offset, int amount);
        int Get(byte[] buffer, int targetOffset, int amount);
        int Read(byte[] buffer);
        int Get(byte[] buffer, int targetOffset);
        byte[] ReadBytes(int amount);
        byte[] GetBytes(int amount);

        /// <summary>
        /// Reads and returns a span of size amount.
        /// Position will be advanced by amount.
        /// </summary>
        /// <param name="amount">Amount to read</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Read span of size amount</returns>
        ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true);

        /// <summary>
        /// Reads and returns a span of size amount.
        /// Position will be advanced by amount.
        /// </summary>
        /// <param name="amount">Amount to read</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Read span of size amount</returns>
        ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true);

        /// <summary>
        /// Returns a span of size amount
        /// </summary>
        /// <param name="amount">Amount to get</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Span of size amount</returns>
        ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true);

        /// <summary>
        /// Returns a span of size amount
        /// </summary>
        /// <param name="amount">Amount to get</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Span of size amount</returns>
        ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true);

        /// <summary>
        /// Reads and returns a span of size amount.
        /// Position will be advanced by amount.
        /// </summary>
        /// <param name="amount">Amount to read</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Read span of size amount</returns>
        ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true);

        /// <summary>
        /// Reads and returns a span of size amount.
        /// Position will be advanced by amount.
        /// </summary>
        /// <param name="amount">Amount to read</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Read span of size amount</returns>
        ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true);

        /// <summary>
        /// Returns a span of size amount
        /// </summary>
        /// <param name="amount">Amount to get</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Span of size amount</returns>
        ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true);

        /// <summary>
        /// Returns a span of size amount
        /// </summary>
        /// <param name="amount">Amount to get</param>
        /// <param name="offset">Offset to read from relative to current position</param>
        /// <param name="readSafe">Whether to ensure the span returned will not be modified by subsequent reads</param>
        /// <returns>Span of size amount</returns>
        ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true);
        bool ReadBool();
        bool GetBool();
        bool GetBool(int offset);
        byte ReadUInt8();
        byte GetUInt8();
        byte GetUInt8(int offset);
        ushort ReadUInt16();
        ushort GetUInt16();
        ushort GetUInt16(int offset);
        uint ReadUInt32();
        uint GetUInt32();
        uint GetUInt32(int offset);
        ulong ReadUInt64();
        ulong GetUInt64();
        ulong GetUInt64(int offset);
        sbyte ReadInt8();
        sbyte GetInt8();
        sbyte GetInt8(int offset);
        short ReadInt16();
        short GetInt16();
        short GetInt16(int offset);
        int ReadInt32();
        int GetInt32();
        int GetInt32(int offset);
        long ReadInt64();
        long GetInt64();
        long GetInt64(int offset);
        float ReadFloat();
        float GetFloat();
        float GetFloat(int offset);
        double ReadDouble();
        double GetDouble();
        double GetDouble(int offset);
        string ReadStringUTF8(int amount);
        string GetStringUTF8(int amount);
        string GetStringUTF8(int amount, int offset);
        void WriteTo(Stream stream, int amount);
    }
}
