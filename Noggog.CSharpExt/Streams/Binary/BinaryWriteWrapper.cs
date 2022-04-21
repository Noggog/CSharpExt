#if NETSTANDARD2_0 
#else
using System.Buffers.Binary;

namespace Noggog
{
    public class BinaryWriteWrapper : IBinaryWriteStream
    {
        private readonly BinaryWriter _stream;
        internal bool _dispose;

        public bool IsLittleEndian { get; }

        public BinaryWriteWrapper(Stream stream, bool dispose = true, bool isLittleEndian = true)
        {
            _dispose = dispose;
            IsLittleEndian = isLittleEndian;
            _stream = new BinaryWriter(stream);
        }

        public long Position 
        {
            get => _stream.BaseStream.Position;
            set => _stream.BaseStream.Position = value;
        }

        public long Length => _stream.BaseStream.Length;

        public Stream BaseStream => _stream.BaseStream;

        public void Dispose()
        {
            if (_dispose)
            {
                _stream.Dispose();
            }
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            _stream.Write(buffer);
        }

        public void Write(bool value)
        {
            _stream.Write(value ? (byte)1 : (byte)0);
        }

        public void Write(byte value)
        {
            _stream.Write(value);
        }

        public void Write(ushort value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[2];
                BinaryPrimitives.WriteUInt16BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(uint value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(ulong value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[8];
                BinaryPrimitives.WriteUInt64BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(sbyte value)
        {
            _stream.Write(value);
        }

        public void Write(short value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[2];
                BinaryPrimitives.WriteInt16BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(int value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[4];
                BinaryPrimitives.WriteInt32BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(long value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[8];
                BinaryPrimitives.WriteInt64BigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(float value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[4];
                BinaryPrimitives.WriteSingleBigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(double value)
        {
            if (IsLittleEndian)
            {
                _stream.Write(value);
            }
            else
            {
                Span<byte> buf = stackalloc byte[8];
                BinaryPrimitives.WriteDoubleBigEndian(buf, value);
                _stream.Write(buf);
            }
        }

        public void Write(ReadOnlySpan<char> value)
        {
            Span<byte> buf = stackalloc byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                buf[i] = (byte)value[i];
            }
            _stream.Write(value);
        }
    }
}
#endif
