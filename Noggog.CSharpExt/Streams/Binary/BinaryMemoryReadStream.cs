using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Runtime.InteropServices;
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

        #region IBinaryReadStream
        long IBinaryReadStream.Position { get => _pos; set => SetPosition(checked((int)value)); }
        long IBinaryReadStream.Length => this._data.Length;
        long IBinaryReadStream.Remaining => this._data.Length - this._pos;
        #endregion
        
        [DebuggerStepThrough]
        public BinaryMemoryReadStream(ReadOnlyMemorySlice<byte> data)
        {
            this._data = data;
        }

        public BinaryMemoryReadStream(byte[] data)
        {
            this._data = new MemorySlice<byte>(data);
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

        public ReadOnlySpan<byte> ReadSpan(int amount, int offset)
        {
            _pos += amount + offset;
            return GetSpan(amount, offset: -amount);
        }

        public ReadOnlySpan<byte> ReadSpan(int amount)
        {
            _pos += amount;
            return GetSpan(amount, offset: -amount);
        }

        public ReadOnlySpan<byte> GetSpan(int amount)
        {
            return _data.Span.Slice(_pos, amount);
        }

        public ReadOnlySpan<byte> GetSpan(int amount, int offset)
        {
            return _data.Span.Slice(_pos + offset, amount);
        }

        public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset)
        {
            _pos += amount + offset;
            return GetMemory(amount, offset: -amount);
        }

        public ReadOnlyMemorySlice<byte> ReadMemory(int amount)
        {
            _pos += amount;
            return GetMemory(amount, offset: -amount);
        }

        public ReadOnlyMemorySlice<byte> GetMemory(int amount)
        {
            return _data.Slice(_pos, amount);
        }

        public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset)
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
            return BinaryPrimitives.ReadUInt16LittleEndian(this._data.Span.Slice(_pos - 2));
        }

        public uint ReadUInt32()
        {
            _pos += 4;
            return BinaryPrimitives.ReadUInt32LittleEndian(this._data.Span.Slice(_pos - 4));
        }

        public ulong ReadUInt64()
        {
            _pos += 8;
            return BinaryPrimitives.ReadUInt64LittleEndian(this._data.Span.Slice(_pos - 8));
        }

        public sbyte ReadInt8()
        {
            return (sbyte)_data.Span[_pos++];
        }

        public short ReadInt16()
        {
            _pos += 2;
            return BinaryPrimitives.ReadInt16LittleEndian(this._data.Span.Slice(_pos - 2));
        }

        public int ReadInt32()
        {
            _pos += 4;
            return BinaryPrimitives.ReadInt32LittleEndian(this._data.Span.Slice(_pos - 4));
        }

        public long ReadInt64()
        {
            _pos += 8;
            return BinaryPrimitives.ReadInt64LittleEndian(this._data.Span.Slice(_pos - 8));
        }

        public string ReadStringUTF8(int amount)
        {
            _pos += amount;
            return SpanExt.GetStringUTF8(this._data.Span.Slice(_pos - amount, amount));
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
            return BinaryPrimitives.ReadUInt16LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public uint GetUInt32(int offset)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public ulong GetUInt64(int offset)
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public sbyte GetInt8(int offset)
        {
            return (sbyte)_data.Span[_pos + offset];
        }

        public short GetInt16(int offset)
        {
            return BinaryPrimitives.ReadInt16LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public int GetInt32(int offset)
        {
            return BinaryPrimitives.ReadInt32LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public long GetInt64(int offset)
        {
            return BinaryPrimitives.ReadInt64LittleEndian(this._data.Span.Slice(_pos + offset));
        }

        public float GetFloat(int offset)
        {
            return SpanExt.GetFloat(this._data.Span.Slice(_pos + offset));
        }

        public double GetDouble(int offset)
        {
            return SpanExt.GetDouble(this._data.Span.Slice(_pos + offset));
        }

        public string GetStringUTF8(int amount, int offset)
        {
            return SpanExt.GetStringUTF8(this._data.Span.Slice(_pos + offset, amount));
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
            return BinaryPrimitives.ReadUInt16LittleEndian(this._data.Span.Slice(_pos));
        }

        public uint GetUInt32()
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(this._data.Span.Slice(_pos));
        }

        public ulong GetUInt64()
        {
            return BinaryPrimitives.ReadUInt64LittleEndian(this._data.Span.Slice(_pos));
        }

        public sbyte GetInt8()
        {
            return (sbyte)_data.Span[_pos];
        }

        public short GetInt16()
        {
            return BinaryPrimitives.ReadInt16LittleEndian(this._data.Span.Slice(_pos));
        }

        public int GetInt32()
        {
            return BinaryPrimitives.ReadInt32LittleEndian(this._data.Span.Slice(_pos));
        }

        public long GetInt64()
        {
            return BinaryPrimitives.ReadInt64LittleEndian(this._data.Span.Slice(_pos));
        }

        public float GetFloat()
        {
            return SpanExt.GetFloat(this._data.Span.Slice(_pos));
        }

        public double GetDouble()
        {
            return SpanExt.GetDouble(this._data.Span.Slice(_pos));
        }

        public string GetStringUTF8(int amount)
        {
            return SpanExt.GetStringUTF8(this._data.Span.Slice(_pos, amount));
        }
    }
}
