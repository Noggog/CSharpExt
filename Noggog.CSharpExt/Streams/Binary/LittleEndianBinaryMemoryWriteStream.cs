using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Noggog.Streams.Binary;

internal class LittleEndianBinaryMemoryWriteStream: IBinaryMemoryWriteStream
{
    internal int _pos;
    internal byte[] _data;
    internal bool _isLittleEndian = true;

    public int Position
    {
        get => _pos;
        set => _pos = value;
    }

    public int Length => _data.Length;
    public int Remaining => _data.Length - _pos;
    public bool Complete => _data.Length <= _pos;
    public bool IsLittleEndian => _isLittleEndian;
    
    #region IBinaryWriteStream
    long IBinaryWriteStream.Position { get => _pos; set => _pos = checked((int)value); }
    long IBinaryWriteStream.Length => _data.Length;
    Stream IBinaryWriteStream.BaseStream => throw new NotImplementedException();
    #endregion

    public LittleEndianBinaryMemoryWriteStream(byte[] buffer)
    {
        _data = buffer;
    }

    public void Write(ReadOnlySpan<byte> buffer, int offset, int amount)
    {
        buffer.Slice(offset, amount).CopyTo(_data.AsSpan().Slice(_pos));
        _pos += amount;
    }

    public void Write(ReadOnlySpan<byte> buffer)
    {
        buffer.CopyTo(_data.AsSpan().Slice(_pos));
        _pos += buffer.Length;
    }

    public void Write(bool b)
    {
        if (b)
        {
            _data[_pos++] = 1;
        }
        else
        {
            _data[_pos++] = 0;
        }
    }

    public void Write(byte b)
    {
        _data[_pos++] = b;
    }

    public void Write(ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(ushort);
    }

    public void Write(uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(uint);
    }

    public void Write(ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(ulong);
    }

    public void Write(sbyte value)
    {
        _data[_pos++] = (byte)value;
    }

    public void Write(short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(short);
    }

    public void Write(int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(int);
    }

    public void Write(long value)
    {
        BinaryPrimitives.WriteInt64LittleEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(long);
    }

    public void Write(float value)
    {
#if NETSTANDARD2_0
        unsafe
        {
            BinaryPrimitives.WriteInt32LittleEndian(_data.AsSpan(_pos), *(int*)&value);
        }
#else
        BinaryPrimitives.WriteSingleLittleEndian(_data.AsSpan(_pos), value);
#endif
        _pos += sizeof(float);
    }

    public void Write(double value)
    {
#if NETSTANDARD2_0
        unsafe
        {
            BinaryPrimitives.WriteInt64LittleEndian(_data.AsSpan(_pos), *(long*)&value);
        }
#else
        BinaryPrimitives.WriteDoubleLittleEndian(_data.AsSpan(_pos), value);
#endif
        _pos += sizeof(double);
    }

    public void Write(ReadOnlySpan<char> str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            _data[_pos++] = (byte)str[i];
        }
    }

    public void Dispose()
    {
        _data = [];
    }
}