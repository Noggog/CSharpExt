using System.Buffers.Binary;
using System.Diagnostics;

namespace Noggog.Streams.Binary;

public class BigEndianBinaryMemoryReadStream : BinaryMemoryReadStream
{
    [DebuggerStepThrough]
    public BigEndianBinaryMemoryReadStream(ReadOnlyMemorySlice<byte> data)
        : base(data)
    {
    }

    public BigEndianBinaryMemoryReadStream(byte[] data)
        : base(data)
    {
    }
    
    public override bool IsLittleEndian => false;

    public override ushort ReadUInt16()
    {
        _pos += 2;
        var span = _data.Span.Slice(_pos - 2);
        return BinaryPrimitives.ReadUInt16BigEndian(span); 
    }

    public override uint ReadUInt32()
    {
        _pos += 4;
        var span = _data.Span.Slice(_pos - 4);
        return BinaryPrimitives.ReadUInt32BigEndian(span); 
    }

    public override ulong ReadUInt64()
    {
        _pos += 8;
        var span = _data.Span.Slice(_pos - 8);
        return BinaryPrimitives.ReadUInt64BigEndian(span); 
    }

    public override short ReadInt16()
    {
        _pos += 2;
        var span = _data.Span.Slice(_pos - 2);
        return BinaryPrimitives.ReadInt16BigEndian(span); 
    }

    public override int ReadInt32()
    {
        _pos += 4;
        var span = _data.Span.Slice(_pos - 4);
        return BinaryPrimitives.ReadInt32BigEndian(span); 
    }

    public override long ReadInt64()
    {
        _pos += 8;
        var span = _data.Span.Slice(_pos - 8);
        return BinaryPrimitives.ReadInt64BigEndian(span); 
    }

    public override ushort GetUInt16(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadUInt16BigEndian(span); 
    }

    public override uint GetUInt32(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadUInt32BigEndian(span); 
    }

    public override ulong GetUInt64(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadUInt64BigEndian(span); 
    }
    
    public override short GetInt16(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadInt16BigEndian(span); 
    }

    public override int GetInt32(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadInt32BigEndian(span); 
    }

    public override long GetInt64(int offset)
    {
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadInt64BigEndian(span); 
    }

    public override float GetFloat(int offset)
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadSingleBigEndian(span); 
#endif
    }

    public override double GetDouble(int offset)
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos + offset);
        return BinaryPrimitives.ReadDoubleBigEndian(span); 
#endif
    }
    
    public override ushort GetUInt16()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadUInt16BigEndian(span); 
    }

    public override uint GetUInt32()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadUInt32BigEndian(span); 
    }

    public override ulong GetUInt64()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadUInt64BigEndian(span); 
    }
    
    public override short GetInt16()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadInt16BigEndian(span); 
    }

    public override int GetInt32()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadInt32BigEndian(span); 
    }

    public override long GetInt64()
    {
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadInt64BigEndian(span); 
    }

    public override float GetFloat()
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadSingleBigEndian(span); 
#endif
    }

    public override double GetDouble()
    {
#if NETSTANDARD2_0
            throw new NotImplementedException();
#else
        var span = _data.Span.Slice(_pos);
        return BinaryPrimitives.ReadDoubleBigEndian(span); 
#endif
    }
}