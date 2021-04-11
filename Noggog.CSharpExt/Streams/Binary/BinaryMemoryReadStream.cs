using System;
using System.IO;
using System.Buffers.Binary;
using System.Diagnostics;

namespace Noggog
{
    public class BinaryMemoryReadStream : IBinaryReadStream
    {
        internal int _pos;
        internal ReadOnlyMemorySlice<byte> _data;
        public ReadOnlyMemorySlice<byte> Data => _data;
        public int Position
        {
            get => this._pos;
            set => SetPosition(value);
        }
        public int Length => this._data.Length;
        public int Remaining => this._data.Length - this._pos;
        public bool Complete => this._data.Length <= this._pos;
        public ReadOnlySpan<byte> RemainingSpan => _data.Span.Slice(_pos);
        public ReadOnlyMemorySlice<byte> RemainingMemory => _data.Slice(_pos);
        public int UnderlyingPosition => _data.StartPosition + this.Position;
        public bool IsPersistantBacking => true;
        public bool IsLittleEndian { get; }
        public Stream BaseStream => throw new NotImplementedException();

        #region IBinaryReadStream
        long IBinaryReadStream.Position { get => _pos; set => SetPosition(checked((int)value)); }
        long IBinaryReadStream.Length => this._data.Length;
        long IBinaryReadStream.Remaining => this._data.Length - this._pos;
        #endregion

        [DebuggerStepThrough]
        public BinaryMemoryReadStream(ReadOnlyMemorySlice<byte> data, bool isLittleEndian = true)
        {
            this._data = data;
            IsLittleEndian = isLittleEndian;
        }

        public BinaryMemoryReadStream(byte[] data, bool isLittleEndian = true)
        {
            this._data = new MemorySlice<byte>(data);
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

        public int Read(byte[] buffer, int offset, int amount)
        {
            var ret = Get(buffer, offset, amount);
            _pos += amount;
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

        public bool ReadBool()
        {
            return _data.Span[_pos++] > 0;
        }

        public byte ReadUInt8()
        {
            return _data.Span[_pos++];
        }

        public byte ReadByte()
        {
            return _data.Span[_pos++];
        }

        public ushort ReadUInt16()
        {
            _pos += 2;
            var span = this._data.Span.Slice(_pos - 2);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public uint ReadUInt32()
        {
            _pos += 4;
            var span = this._data.Span.Slice(_pos - 4);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public ulong ReadUInt64()
        {
            _pos += 8;
            var span = this._data.Span.Slice(_pos - 8);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
        }

        public sbyte ReadInt8()
        {
            return (sbyte)_data.Span[_pos++];
        }

        public short ReadInt16()
        {
            _pos += 2;
            var span = this._data.Span.Slice(_pos - 2);
            return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public int ReadInt32()
        {
            _pos += 4;
            var span = this._data.Span.Slice(_pos - 4);
            return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public long ReadInt64()
        {
            _pos += 8;
            var span = this._data.Span.Slice(_pos - 8);
            return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public string ReadStringUTF8(int amount)
        {
            _pos += amount;
            return SpanExt.StringUTF8(this._data.Span.Slice(_pos - amount, amount));
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

        public void Dispose()
        {
            this._data = default;
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

        public bool GetBool(int offset)
        {
            return _data.Span[_pos + offset] > 0;
        }

        public byte GetUInt8(int offset)
        {
            return _data.Span[_pos + offset];
        }

        public ushort GetUInt16(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public uint GetUInt32(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public ulong GetUInt64(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
        }

        public sbyte GetInt8(int offset)
        {
            return (sbyte)_data.Span[_pos + offset];
        }

        public short GetInt16(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public int GetInt32(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public long GetInt64(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public float GetFloat(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadSingleLittleEndian(span) : BinaryPrimitives.ReadSingleBigEndian(span);
        }

        public double GetDouble(int offset)
        {
            var span = this._data.Span.Slice(_pos + offset);
            return IsLittleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(span) : BinaryPrimitives.ReadDoubleBigEndian(span);
        }

        public string GetStringUTF8(int amount, int offset)
        {
            return SpanExt.StringUTF8(this._data.Span.Slice(_pos + offset, amount));
        }

        public bool GetBool()
        {
            return _data.Span[_pos] > 0;
        }

        public byte GetUInt8()
        {
            return _data.Span[_pos];
        }

        public ushort GetUInt16()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(span) : BinaryPrimitives.ReadUInt16BigEndian(span);
        }

        public uint GetUInt32()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt32LittleEndian(span) : BinaryPrimitives.ReadUInt32BigEndian(span);
        }

        public ulong GetUInt64()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadUInt64LittleEndian(span) : BinaryPrimitives.ReadUInt64BigEndian(span);
        }

        public sbyte GetInt8()
        {
            return (sbyte)_data.Span[_pos];
        }

        public short GetInt16()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadInt16LittleEndian(span) : BinaryPrimitives.ReadInt16BigEndian(span);
        }

        public int GetInt32()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadInt32LittleEndian(span) : BinaryPrimitives.ReadInt32BigEndian(span);
        }

        public long GetInt64()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadInt64LittleEndian(span) : BinaryPrimitives.ReadInt64BigEndian(span);
        }

        public float GetFloat()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadSingleLittleEndian(span) : BinaryPrimitives.ReadSingleBigEndian(span);
        }

        public double GetDouble()
        {
            var span = this._data.Span.Slice(_pos);
            return IsLittleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(span) : BinaryPrimitives.ReadDoubleBigEndian(span);
        }

        public string GetStringUTF8(int amount)
        {
            return SpanExt.StringUTF8(this._data.Span.Slice(_pos, amount));
        }
    }
}
