using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class BinaryReadStream : Stream, IBinaryReadStream
    {
        public const int NearnessLength = 20;
        public static readonly byte[] NearnessBuffer = new byte[NearnessLength];
        public readonly Stream _stream;
        internal readonly byte[] _data;
        internal long _streamPos;
        internal readonly long _length;
        internal bool CompleteBuffering => _streamPos == _length;
        internal readonly BinaryMemoryReadStream _internalMemoryStream;
        internal bool _dispose;
        internal int _internalBufferLength;
        internal int _posOffset;
        private int InternalStreamRemaining => _internalMemoryStream.Remaining - _posOffset;
        private bool forceReload;

        public override long Length => _length;
        public long Remaining => _length - this.Position;
        public bool Complete => _length <= this.Position;
        public override long Position { get => _streamPos - InternalStreamRemaining; set => SetPosition(value); }

        public bool CheckUnderlyingStreamPosition = false;

        public BinaryReadStream(Stream stream, int bufferSize = 4096, bool dispose = true)
        {
            if (stream.Position != 0)
            {
                throw new NotImplementedException("Stream must start at position zero.");
            }
            this._dispose = dispose;
            this._stream = stream;
            this._length = this._stream.Length;
            this._data = new byte[bufferSize];
            this._internalMemoryStream = new BinaryMemoryReadStream(this._data);
            this._internalMemoryStream.Position = _data.Length;
            this._internalBufferLength = _data.Length;
        }

        public BinaryReadStream(string path, int bufferSize = 4096)
            : this(File.OpenRead(path), bufferSize)
        {
        }

        private void SetPosition(long pos)
        {
            if (pos < 0)
            {
                throw new ArgumentException("Cannot move position to a negative value.");
            }
            var startLoc = _streamPos - _internalBufferLength;
            if (pos > _streamPos
                || pos < startLoc
                || forceReload)
            {
                forceReload = false;
                if (pos != _streamPos)
                {
                    _internalMemoryStream.Position = _internalMemoryStream.Length;
                    var diff = pos - _streamPos;
                    if (diff > 0
                        && diff < NearnessLength)
                    {
                        _stream.Read(NearnessBuffer, 0, (int)diff);
                    }
                    else
                    {
                        _stream.Position = pos;
                    }
                    _streamPos = pos;
                }
                // ToDo
                // Maybe remove
                LoadPosition();
            }
            else
            {
                _internalMemoryStream.Position = checked((int)(pos - _streamPos + _internalBufferLength));
            }
            if (this.Position != pos)
            {
                throw new ArgumentException();
            }
            if (CheckUnderlyingStreamPosition 
                && _streamPos != _stream.Position)
            {
                throw new ArgumentException();
            }
        }

        internal void LoadPosition()
        {
            if (_internalMemoryStream.Complete)
            {
                var amountRead = _stream.Read(_data, 0, _data.Length);
                _streamPos += amountRead;
                _internalBufferLength = amountRead;
            }
            else
            {
                var diff = _internalMemoryStream.Remaining;
                var buffDiff = (int)(_data.Length - diff);
                Array.Copy(_data, buffDiff, _data, 0, diff);
                var amountRead = _stream.Read(_data, (int)diff, buffDiff);
                _streamPos += amountRead;
                _internalBufferLength = diff + amountRead;
            }
            _internalMemoryStream.Position = 0;
            _posOffset = _data.Length - _internalBufferLength;
            if (_streamPos != _stream.Position)
            {
                throw new ArgumentException();
            }
        }

        private void LoadPosition(int amount)
        {
            if (InternalStreamRemaining < amount)
            {
                LoadPosition();
            }
        }

        public int Read(byte[] buffer)
        {
            return Read(buffer, offset: 0, amount: buffer.Length);
        }

        public override int Read(byte[] buffer, int offset, int amount)
        {
            if (amount < InternalStreamRemaining)
            {
                return _internalMemoryStream.Read(buffer, offset, amount);
            }
            if (CompleteBuffering)
            {
                amount = InternalStreamRemaining;
                Array.Copy(_data, _internalMemoryStream.Position, buffer, offset, amount);
                _internalMemoryStream.Position += amount;
                return amount;
            }

            // Copy remaining
            var remaining = InternalStreamRemaining;
            if (remaining > 0)
            {
                Array.Copy(_data, _internalMemoryStream.Position, buffer, offset, remaining);
                _internalMemoryStream.Position += remaining;
            }


            offset += remaining;
            amount -= remaining;

            if (amount <= _data.Length)
            {
                LoadPosition();
                return remaining + Read(buffer, offset, amount);
            }
            else
            {
                forceReload = true;
                _stream.Read(buffer, offset, amount);
                _streamPos += amount;
                return amount + remaining;
            }
        }

        public byte[] ReadBytes(int amount)
        {
            var ret = new byte[amount];
            if (amount != Read(ret, offset: 0, amount: amount))
            {
                throw new IndexOutOfRangeException();
            }
            return ret;
        }

        public bool ReadBool()
        {
            LoadPosition(1);
            return _internalMemoryStream.ReadBool();
        }

        public byte ReadUInt8()
        {
            LoadPosition(1);
            return _internalMemoryStream.ReadUInt8();
        }

        public override int ReadByte()
        {
            LoadPosition(1);
            return _internalMemoryStream.ReadUInt8();
        }

        public ushort ReadUInt16()
        {
            LoadPosition(2);
            return _internalMemoryStream.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            LoadPosition(4);
            return _internalMemoryStream.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            LoadPosition(8);
            return _internalMemoryStream.ReadUInt64();
        }

        public sbyte ReadInt8()
        {
            LoadPosition(1);
            return _internalMemoryStream.ReadInt8();
        }

        public short ReadInt16()
        {
            LoadPosition(2);
            return _internalMemoryStream.ReadInt16();
        }

        public int ReadInt32()
        {
            LoadPosition(4);
            return _internalMemoryStream.ReadInt32();
        }

        public long ReadInt64()
        {
            LoadPosition(8);
            return _internalMemoryStream.ReadInt64();
        }

        public float ReadFloat()
        {
            LoadPosition(4);
            return _internalMemoryStream.ReadFloat();
        }

        public double ReadDouble()
        {
            LoadPosition(4);
            return _internalMemoryStream.ReadDouble();
        }

        public string ReadString(int amount)
        {
            if (amount <= InternalStreamRemaining)
            {
                return _internalMemoryStream.ReadString(amount);
            }
            if (amount < _data.Length)
            {
                LoadPosition();
                return _internalMemoryStream.ReadString(amount);
            }

            forceReload = true;
            byte[] arr = new byte[amount];
            var numRead = _internalMemoryStream.Read(arr, 0, InternalStreamRemaining);

            amount -= numRead;

            _stream.Read(arr, numRead, amount);
            _streamPos += amount;
            return BinaryUtility.BytesToString(arr, 0, amount + numRead);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this._dispose)
            {
                this._stream.Dispose();
            }
            this._internalMemoryStream.Dispose();
        }

        #region Stream
        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public void WriteTo(Stream stream, int amount)
        {
            while (amount > 0)
            {
                LoadPosition(amount);
                var internalRemaining = InternalStreamRemaining;
                var toRead = amount < internalRemaining ? amount : internalRemaining;
                _internalMemoryStream.WriteTo(stream, toRead);
                amount -= toRead;
            }
        }
        #endregion
    }
}
