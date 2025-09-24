using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace Noggog;

public class BigEndianBinaryMemoryWriteStream: IBinaryMemoryWriteStream
{
    internal int _pos;
    internal byte[] _data;
    internal bool _isLittleEndian = false;

    public int Position
    {
        get => _pos;
        set => _pos = value;
    }

    public int Length => _data.Length;
    public int Remaining => _data.Length - _pos;
    public bool Complete => _data.Length <= _pos;
    public bool IsLittleEndian { get; }
    
    #region IBinaryWriteStream
    long IBinaryWriteStream.Position { get => _pos; set => _pos = checked((int)value); }
    long IBinaryWriteStream.Length => _data.Length;
    Stream IBinaryWriteStream.BaseStream => throw new NotImplementedException();
    #endregion

    public BigEndianBinaryMemoryWriteStream(byte[] buffer)
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
        BinaryPrimitives.WriteUInt16BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(ushort);
    }

    public void Write(uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(uint);
    }

    public void Write(ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(ulong);
    }

    public void Write(sbyte value)
    {
        _data[_pos++] = (byte)value;
    }

    public void Write(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(short);
    }

    public void Write(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(int);
    }

    public void Write(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(_data.AsSpan(_pos), value);
        _pos += sizeof(long);
    }

    public void Write(float value)
    {
#if NETSTANDARD2_0
        // Suggested code.  Needs testing
        // unsafe
        // {
        //     BinaryPrimitives.WriteInt32LittleEndian(_data.AsSpan(_pos), *(int*)&value);
        // }
        Write(new Int32SingleUnion(value).AsInt32);
#else
        BinaryPrimitives.WriteSingleBigEndian(_data.AsSpan(_pos), value);
#endif
        _pos += sizeof(float);
    }

    public void Write(double value)
    {
#if NETSTANDARD2_0
        // Suggested code.  Needs testing
        // unsafe
        // {
        //     BinaryPrimitives.WriteInt64LittleEndian(_data.AsSpan(_pos), *(long*)&value);
        // }
        Write(BitConverter.DoubleToInt64Bits(value));
#else
        BinaryPrimitives.WriteDoubleBigEndian(_data.AsSpan(_pos), value);
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
        _data = Array.Empty<byte>();
    }
    
    #region Private struct used for Single/Int32 conversions

    /// <summary>
    /// Taken from http://jonskeet.uk/csharp/miscutil/
    /// Union used solely for the equivalent of DoubleToInt64Bits and vice versa.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    struct Int32SingleUnion
    {
        /// <summary>
        /// Int32 version of the value.
        /// </summary>
        [FieldOffset(0)] int i;

        /// <summary>
        /// Single version of the value.
        /// </summary>
        [FieldOffset(0)] float f;

        /// <summary>
        /// Creates an instance representing the given integer.
        /// </summary>
        /// <param name="i">The integer value of the new instance.</param>
        internal Int32SingleUnion(int i)
        {
            f = 0; // Just to keep the compiler happy
            this.i = i;
        }

        /// <summary>
        /// Creates an instance representing the given floating point number.
        /// </summary>
        /// <param name="f">The floating point value of the new instance.</param>
        internal Int32SingleUnion(float f)
        {
            i = 0; // Just to keep the compiler happy
            this.f = f;
        }

        /// <summary>
        /// Returns the value of the instance as an integer.
        /// </summary>
        internal int AsInt32
        {
            get { return i; }
        }

        /// <summary>
        /// Returns the value of the instance as a floating point number.
        /// </summary>
        internal float AsSingle
        {
            get { return f; }
        }
    }

    #endregion
}