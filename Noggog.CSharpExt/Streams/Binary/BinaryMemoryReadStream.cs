using System.Diagnostics;
using Noggog.Streams.Binary;

namespace Noggog;

public abstract class BinaryMemoryReadStream : Stream, IBinaryReadStream
{
    public static BinaryMemoryReadStream Factory(byte[] buffer, bool isLittleEndian)
    {
        return isLittleEndian
            ? new LittleEndianBinaryMemoryReadStream(buffer)
            : new BigEndianBinaryMemoryReadStream(buffer);
    }
    
    public static BinaryMemoryReadStream Factory(ReadOnlyMemorySlice<byte> data, bool isLittleEndian)
    {
        return isLittleEndian
            ? new LittleEndianBinaryMemoryReadStream(data)
            : new BigEndianBinaryMemoryReadStream(data);
    }
    
    public static BinaryMemoryReadStream BigEndian(byte[] buffer)
    {
        return new BigEndianBinaryMemoryReadStream(buffer);
    }
    
    public static BinaryMemoryReadStream BigEndian(ReadOnlyMemorySlice<byte> data)
    {
        return new BigEndianBinaryMemoryReadStream(data);
    }
    
    public static BinaryMemoryReadStream LittleEndian(byte[] buffer)
    {
        return new LittleEndianBinaryMemoryReadStream(buffer);
    }
    
    public static BinaryMemoryReadStream LittleEndian(ReadOnlyMemorySlice<byte> data)
    {
        return new LittleEndianBinaryMemoryReadStream(data);
    }
    
    internal int _pos;
    internal ReadOnlyMemorySlice<byte> _data;
    public ReadOnlyMemorySlice<byte> Data => _data;

    public override long Position
    {
        get => _pos;
        set => SetPosition(checked((int)value));
    }

    public int PositionInt
    {
        get => _pos;
        set => _pos = value;
    }

    public int Remaining => _data.Length - _pos;
    public bool Complete => _data.Length <= _pos;
    public abstract bool IsLittleEndian { get; }
    public ReadOnlySpan<byte> RemainingSpan => _data.Span.Slice(_pos);
    public ReadOnlyMemorySlice<byte> RemainingMemory => _data.Slice(_pos);
    public int UnderlyingPosition => _data.StartPosition + _pos;
    public bool IsPersistantBacking => true;
    public Stream BaseStream => this;

    #region IBinaryReadStream

    long IBinaryReadStream.Position
    {
        get => _pos;
        set => SetPosition(checked((int)value));
    }

    long IBinaryReadStream.Length => _data.Length;
    long IBinaryReadStream.Remaining => _data.Length - _pos;
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _data.Length;
    public int LengthInt => _data.Length;

    #endregion

    [DebuggerStepThrough]
    protected BinaryMemoryReadStream(ReadOnlyMemorySlice<byte> data)
    {
        _data = data;
    }

    protected BinaryMemoryReadStream(byte[] data)
    {
        _data = new MemorySlice<byte>(data);
    }

    private void SetPosition(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Cannot set to a negative position");
        }

        _pos = value;
    }

    public int Read(byte[] buffer)
    {
        return Read(buffer, offset: 0, amount: buffer.Length);
    }

    public override int Read(byte[] buffer, int offset, int amount)
    {
        var ret = Get(buffer, offset, amount);
        _pos += ret;
        return ret;
    }

    public byte[] GetBytes(int amount)
    {
        return _data.Span.Slice(_pos, amount).ToArray();
    }

    public byte[] ReadBytes(int amount)
    {
        var ret = GetBytes(amount);
        _pos += amount;
        return ret;
    }

    public ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true)
    {
        _pos += amount + offset;
        return GetSpan(amount, offset: -amount);
    }

    public ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true)
    {
        _pos += amount;
        return GetSpan(amount, offset: -amount);
    }

    public ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true)
    {
        return _data.Span.Slice(_pos, amount);
    }

    public ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true)
    {
        return _data.Span.Slice(_pos + offset, amount);
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true)
    {
        _pos += amount + offset;
        return GetMemory(amount, offset: -amount);
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true)
    {
        _pos += amount;
        return GetMemory(amount, offset: -amount);
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true)
    {
        return _data.Slice(_pos, amount);
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true)
    {
        return _data.Slice(_pos + offset, amount);
    }

    public bool ReadBoolean()
    {
        return _data.Span[_pos++] > 0;
    }

    public byte ReadUInt8()
    {
        return _data.Span[_pos++];
    }

    public override int ReadByte()
    {
        return _data.Span[_pos++];
    }

    public abstract ulong GetUInt64(int offset);

    public sbyte ReadInt8()
    {
        return (sbyte)_data.Span[_pos++];
    }

    public abstract double GetDouble(int offset);

    public string ReadStringUTF8(int amount)
    {
        _pos += amount;
        return SpanExt.StringUTF8(_data.Span.Slice(_pos - amount, amount));
    }

    public float ReadFloat()
    {
        _pos += 4;
        return GetFloat(offset: -4);
    }

    public double ReadDouble()
    {
        _pos += 8;
        return GetDouble(offset: -8);
    }

    protected override void Dispose(bool disposing)
    {
        _data = default;
        base.Dispose(disposing);
    }

    public void WriteTo(Stream stream, int amount)
    {
        // ToDo
        // Swap to direct span usage when .NET Standard 2.1 comes out
        var arr = _data.Span.Slice(_pos, amount).ToArray();
        stream.Write(arr, 0, arr.Length);
        _pos += amount;
    }

    public int Get(byte[] buffer, int targetOffset, int amount)
    {
        if (amount > Remaining)
        {
            amount = Remaining;
        }

        _data.Span.Slice(_pos, amount).CopyTo(buffer.AsSpan(targetOffset));
        return amount;
    }

    public int Get(byte[] buffer, int targetOffset)
    {
        return Get(buffer, targetOffset: targetOffset, amount: buffer.Length);
    }

    public bool GetBoolean(int offset)
    {
        return _data.Span[_pos + offset] > 0;
    }

    public byte GetUInt8(int offset)
    {
        return _data.Span[_pos + offset];
    }

    public abstract ushort ReadUInt16();
    public abstract ushort GetUInt16();
    public abstract ushort GetUInt16(int offset);
    public abstract uint ReadUInt32();
    public abstract uint GetUInt32();
    public abstract uint GetUInt32(int offset);
    public abstract ulong ReadUInt64();
    public abstract ulong GetUInt64();

    public sbyte GetInt8(int offset)
    {
        return (sbyte)_data.Span[_pos + offset];
    }

    public abstract short ReadInt16();
    public abstract short GetInt16();
    public abstract short GetInt16(int offset);
    public abstract int ReadInt32();
    public abstract int GetInt32();
    public abstract int GetInt32(int offset);
    public abstract long ReadInt64();
    public abstract long GetInt64();
    public abstract long GetInt64(int offset);
    public abstract float GetFloat();
    public abstract float GetFloat(int offset);
    public abstract double GetDouble();

    public string GetStringUTF8(int amount, int offset)
    {
        return SpanExt.StringUTF8(_data.Span.Slice(_pos + offset, amount));
    }

    public bool GetBoolean()
    {
        return _data.Span[_pos] > 0;
    }

    public byte GetUInt8()
    {
        return _data.Span[_pos];
    }

    public sbyte GetInt8()
    {
        return (sbyte)_data.Span[_pos];
    }

    public string GetStringUTF8(int amount)
    {
        return SpanExt.StringUTF8(_data.Span.Slice(_pos, amount));
    }

    public override void Flush()
    {
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Length + offset;
                break;
            default:
                throw new NotImplementedException();
        }

        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _data.Span.Slice(_pos, count).CopyTo(buffer.AsSpan().Slice(offset));
    }
}