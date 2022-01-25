using System;
using System.IO;

namespace Noggog;

public interface IBinaryWriteStream : IDisposable
{
    long Position { get; set; }
    long Length { get; }
    bool IsLittleEndian { get; }
    Stream BaseStream { get; }
#if NETSTANDARD2_0
#else
    void Write(ReadOnlySpan<byte> buffer);
#endif
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
}