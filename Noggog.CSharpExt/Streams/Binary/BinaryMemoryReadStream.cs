using System.Buffers.Binary;
using System.Diagnostics;

namespace Noggog;

public class BinaryMemoryReadStream : Stream, IBinaryReadStream
{
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
    public ReadOnlySpan<byte> RemainingSpan => _data.Span.Slice(_pos);
    public ReadOnlyMemorySlice<byte> RemainingMemory => _data.Slice(_pos);
    public int UnderlyingPosition => _data.StartPosition + _pos;
    public bool IsPersistantBacking => true;
    public bool IsLittleEndian { get; }
    public Stream BaseStream => this;

    #region IBinaryReadStream
    long IBinaryReadStream.Position { get => _pos; set => SetPosition(checked((int)value)); }
    long IBinaryReadStream.Length => _data.Length;
    long IBinaryReadStream.Remaining => _data.Length - _pos;
    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _data.Length;
    public int LengthInt => _data.Length;
    #endregion

    [DebuggerStepThrough]
    public BinaryMemoryReadStream(ReadOnlyMemorySlice<byte> data, bool isLittleEndian = true)
    {
        _data = data;
        IsLittleEndian = isLittleEndian;
    }

    public BinaryMemoryReadStream(byte[] data, bool isLittleEndian = true)
    {
        _data = new MemorySlice<byte>(data);
        IsLittleEndian = isLittleEndian;
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

    public ushort ReadUInt16()
    {
        _pos += 2;
        var span = _data.Span.Slice(_pos - 2);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public uint ReadUInt32()
    {
        _pos += 4;
        var span = _data.Span.Slice(_pos - 4);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
    }

    public ulong ReadUInt64()
    {
        _pos += 8;
        var span = _data.Span.Slice(_pos - 8);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
    }

    public sbyte ReadInt8()
    {
        return (sbyte)_data.Span[_pos++];
    }

    public short ReadInt16()
    {
        _pos += 2;
        var span = _data.Span.Slice(_pos - 2);
        return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
    }

    public int ReadInt32()
    {
        _pos += 4;
        var span = _data.Span.Slice(_pos - 4);
        return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
    }

    public long ReadInt64()
    {
        _pos += 8;
        var span = _data.Span.Slice(_pos - 8);
        return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
    }

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

    public ushort GetUInt16(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public uint GetUInt32(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
    }

    public ulong GetUInt64(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
    }

    public sbyte GetInt8(int offset)
    {
        return (sbyte)_data.Span[_pos + offset];
    }

    public short GetInt16(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
    }

    public int GetInt32(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
    }

    public long GetInt64(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
    }

    public float GetFloat(int offset)
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadSingleLittleEndian(span) : BinaryPrimitives.ReadSingleBigEndian(span);
#endif
    }

    public double GetDouble(int offset)
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos + offset);
        return IsLittleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(span) : BinaryPrimitives.ReadDoubleBigEndian(span);
#endif
    }

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

    public ushort GetUInt16()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
    }

    public uint GetUInt32()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
    }

    public ulong GetUInt64()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
    }

    public sbyte GetInt8()
    {
        return (sbyte)_data.Span[_pos];
    }

    public short GetInt16()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
    }

    public int GetInt32()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
    }

    public long GetInt64()
    {
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
    }

    public float GetFloat()
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadSingleLittleEndian(span) : BinaryPrimitives.ReadSingleBigEndian(span);
#endif
    }

    public double GetDouble()
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos);
        return IsLittleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(span) : BinaryPrimitives.ReadDoubleBigEndian(span);
#endif
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