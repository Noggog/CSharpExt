using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class BinaryMemoryWriteStream : IBinaryWriteStream
    {
        internal int _pos;
        internal byte[] _data;
        public int Position { get => this._pos; set => _pos = value; }
        public int Length => this._data.Length;
        public int Remaining => this._data.Length - this._pos;
        public bool Complete => this._data.Length <= this._pos;
        public bool IsLittleEndian { get; }

        #region IBinaryWriteStream
        long IBinaryWriteStream.Position { get => _pos; set => _pos = checked((int)value); }
        long IBinaryWriteStream.Length => this._data.Length;
        Stream IBinaryWriteStream.BaseStream => throw new NotImplementedException();
        #endregion

        public BinaryMemoryWriteStream(byte[] buffer, bool isLittleEndian = true)
        {
            this._data = buffer;
            IsLittleEndian = isLittleEndian;
        }

        public void Write(ReadOnlySpan<byte> buffer, int offset, int amount)
        {
            buffer.Slice(offset, amount).CopyTo(_data.AsSpan().Slice(_pos));
        }

        public void Write(ReadOnlySpan<byte> buffer)
        {
            buffer.CopyTo(_data.AsSpan().Slice(_pos));
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
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(uint value)
        {
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 0x18);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(ulong value)
        {
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x20);
                _data[_pos++] = (byte)(value >> 0x28);
                _data[_pos++] = (byte)(value >> 0x30);
                _data[_pos++] = (byte)(value >> 0x38);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 0x38);
                _data[_pos++] = (byte)(value >> 0x30);
                _data[_pos++] = (byte)(value >> 0x28);
                _data[_pos++] = (byte)(value >> 0x20);
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(sbyte value)
        {
            _data[_pos++] = (byte)value;
        }

        public void Write(short value)
        {
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(int value)
        {
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 0x18);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(long value)
        {
            if (IsLittleEndian)
            {
                _data[_pos++] = (byte)value;
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x20);
                _data[_pos++] = (byte)(value >> 0x28);
                _data[_pos++] = (byte)(value >> 0x30);
                _data[_pos++] = (byte)(value >> 0x38);
            }
            else
            {
                _data[_pos++] = (byte)(value >> 0x38);
                _data[_pos++] = (byte)(value >> 0x30);
                _data[_pos++] = (byte)(value >> 0x28);
                _data[_pos++] = (byte)(value >> 0x20);
                _data[_pos++] = (byte)(value >> 0x18);
                _data[_pos++] = (byte)(value >> 0x10);
                _data[_pos++] = (byte)(value >> 8);
                _data[_pos++] = (byte)value;
            }
        }

        public void Write(float value)
        {
            Write(new Int32SingleUnion(value).AsInt32);
        }

        public void Write(double value)
        {
            Write(BitConverter.DoubleToInt64Bits(value));
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
            _data = new byte[0];
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
            [FieldOffset(0)]
            int i;
            /// <summary>
            /// Single version of the value.
            /// </summary>
            [FieldOffset(0)]
            float f;

            /// <summary>
            /// Creates an instance representing the given integer.
            /// </summary>
            /// <param name="i">The integer value of the new instance.</param>
            internal Int32SingleUnion(int i)
            {
                this.f = 0; // Just to keep the compiler happy
                this.i = i;
            }

            /// <summary>
            /// Creates an instance representing the given floating point number.
            /// </summary>
            /// <param name="f">The floating point value of the new instance.</param>
            internal Int32SingleUnion(float f)
            {
                this.i = 0; // Just to keep the compiler happy
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
}
