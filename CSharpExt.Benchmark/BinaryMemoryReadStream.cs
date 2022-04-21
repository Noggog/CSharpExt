using BenchmarkDotNet.Attributes;

namespace CSharpExt.Benchmark;

[MemoryDiagnoser]
public class BinaryMemoryReadStream
{
    static byte[] data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
    static byte[] largeData;
    const int offset = 3;
    byte[] dataTarget = new byte[data.Length];
    byte[] offsetDataTarget = new byte[data.Length + offset];
    byte[] largeTarget = new byte[largeData.Length];
    Noggog.BinaryMemoryReadStream readStream = new Noggog.BinaryMemoryReadStream(data);
    Noggog.BinaryMemoryReadStream largeReadStream = new Noggog.BinaryMemoryReadStream(largeData);

    static BinaryMemoryReadStream()
    {
        largeData = new byte[4096];
        for (int i = 0; i < largeData.Length; i++)
        {
            largeData[i] = (byte)(i % 256);
        }
    }

    [Benchmark]
    public int Get()
    {
        return readStream.Get(buffer: dataTarget, targetOffset: 0);
    }

    [Benchmark]
    public int GetLarge()
    {
        return readStream.Get(buffer: largeTarget, targetOffset: 0);
    }

    [Benchmark]
    public int GetOffsetAmount()
    {
        return readStream.Get(buffer: dataTarget, targetOffset: offset, amount: 6);
    }

    [Benchmark]
    public int GetOffset()
    {
        return readStream.Get(buffer: offsetDataTarget, targetOffset: offset);
    }

    [Benchmark]
    public int Read()
    {
        readStream.Position = 0;
        return readStream.Read(dataTarget);
    }

    [Benchmark]
    public int ReadLarge()
    {
        readStream.Position = 0;
        return readStream.Read(largeTarget);
    }

    [Benchmark]
    public int ReadOffsetAmount()
    {
        readStream.Position = 0;
        return readStream.Read(dataTarget, offset: offset, amount: 6);
    }

    [Benchmark]
    public byte[] GetBytes()
    {
        return readStream.GetBytes(data.Length);
    }

    [Benchmark]
    public byte[] ReadBytes()
    {
        readStream.Position = 0;
        return readStream.ReadBytes(data.Length);
    }

    [Benchmark]
    public ReadOnlySpan<byte> GetSpan()
    {
        return readStream.GetSpan(data.Length);
    }

    [Benchmark]
    public ReadOnlySpan<byte> ReadSpan()
    {
        readStream.Position = 0;
        return readStream.ReadSpan(data.Length);
    }

    [Benchmark]
    public byte[] GetSpanBytes()
    {
        return readStream.GetSpan(data.Length).ToArray();
    }

    [Benchmark]
    public byte[] ReadSpanBytes()
    {
        readStream.Position = 0;
        return readStream.ReadSpan(data.Length).ToArray();
    }

    [Benchmark]
    public bool GetBool()
    {
        return readStream.GetBoolean();
    }

    [Benchmark]
    public bool GetBoolOffset()
    {
        return readStream.GetBoolean(offset: offset);
    }

    [Benchmark]
    public bool ReadBool()
    {
        readStream.Position = 0;
        return readStream.ReadBoolean();
    }

    [Benchmark]
    public byte GetUInt8()
    {
        return readStream.GetUInt8();
    }

    [Benchmark]
    public byte GetUInt8Offset()
    {
        return readStream.GetUInt8(offset: offset);
    }

    [Benchmark]
    public byte ReadUInt8()
    {
        readStream.Position = 0;
        return readStream.ReadUInt8();
    }

    [Benchmark]
    public ushort GetUInt16()
    {
        return readStream.GetUInt16();
    }

    [Benchmark]
    public ushort GetUInt16Offset()
    {
        return readStream.GetUInt16(offset: offset);
    }

    [Benchmark]
    public ushort ReadUInt16()
    {
        readStream.Position = 0;
        return readStream.ReadUInt16();
    }

    [Benchmark]
    public uint GetUInt32()
    {
        return readStream.GetUInt32();
    }

    [Benchmark]
    public uint GetUInt32Offset()
    {
        return readStream.GetUInt32(offset: offset);
    }

    [Benchmark]
    public uint ReadUInt32()
    {
        readStream.Position = 0;
        return readStream.ReadUInt32();
    }

    [Benchmark]
    public ulong GetUInt64()
    {
        return readStream.GetUInt64();
    }

    [Benchmark]
    public ulong GetUInt64Offset()
    {
        return readStream.GetUInt64(offset: offset);
    }

    [Benchmark]
    public ulong ReadUInt64()
    {
        readStream.Position = 0;
        return readStream.ReadUInt64();
    }

    [Benchmark]
    public sbyte GetInt8()
    {
        return readStream.GetInt8();
    }

    [Benchmark]
    public sbyte GetInt8Offset()
    {
        return readStream.GetInt8(offset: offset);
    }

    [Benchmark]
    public sbyte ReadInt8()
    {
        readStream.Position = 0;
        return readStream.ReadInt8();
    }

    [Benchmark]
    public short GetInt16()
    {
        return readStream.GetInt16();
    }

    [Benchmark]
    public short GetInt16Offset()
    {
        return readStream.GetInt16(offset: offset);
    }

    [Benchmark]
    public short ReadInt16()
    {
        readStream.Position = 0;
        return readStream.ReadInt16();
    }

    [Benchmark]
    public int GetInt32()
    {
        return readStream.GetInt32();
    }

    [Benchmark]
    public int GetInt32Offset()
    {
        return readStream.GetInt32(offset: offset);
    }

    [Benchmark]
    public int ReadInt32()
    {
        readStream.Position = 0;
        return readStream.ReadInt32();
    }

    [Benchmark]
    public long GetInt64()
    {
        return readStream.GetInt64();
    }

    [Benchmark]
    public long GetInt64Offset()
    {
        return readStream.GetInt64(offset: offset);
    }

    [Benchmark]
    public long ReadInt64()
    {
        readStream.Position = 0;
        return readStream.ReadInt64();
    }

    [Benchmark]
    public string GetStringUTF8()
    {
        return readStream.GetStringUTF8(amount: 8);
    }

    [Benchmark]
    public string GetStringUTF8Offset()
    {
        return readStream.GetStringUTF8(amount: 8, offset: offset);
    }

    [Benchmark]
    public string ReadStringUTF8()
    {
        readStream.Position = 0;
        return readStream.ReadStringUTF8(amount: 8);
    }

    [Benchmark]
    public float GetFloat()
    {
        return readStream.GetFloat();
    }

    [Benchmark]
    public float GetFloatOffset()
    {
        return readStream.GetFloat(offset: offset);
    }

    [Benchmark]
    public float ReadFloat()
    {
        readStream.Position = 0;
        return readStream.ReadFloat();
    }

    [Benchmark]
    public double GetDouble()
    {
        return readStream.GetDouble();
    }

    [Benchmark]
    public double GetDoubleOffset()
    {
        return readStream.GetDouble(offset: offset);
    }

    [Benchmark]
    public double ReadDouble()
    {
        readStream.Position = 0;
        return readStream.GetDouble();
    }
}