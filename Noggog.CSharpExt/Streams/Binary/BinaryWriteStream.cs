using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    /// <summary>
    /// Not finished or tested
    /// </summary>
    public class BinaryWriteStream : Stream, IBinaryWriteStream
    {
        public readonly Stream _stream;
        internal readonly byte[] _data;
        internal bool _dispose;
        internal long _streamFrontlinePos;
        internal long _pos;
        internal readonly BinaryMemoryWriteStream _internalMemoryStream;
        private int InternalStreamRemaining => _internalMemoryStream.Remaining;

        public override long Position
        {
            get => _pos;
            set => SetPosition(value);
        }

        public override long Length => _streamFrontlinePos + _internalMemoryStream.Position;

        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public bool IsLittleEndian { get; }

        Stream IBinaryWriteStream.BaseStream => this;

        public BinaryWriteStream(Stream stream, int bufferSize = 4096, bool dispose = true, bool isLittleEndian = true)
        {
            this._dispose = dispose;
            this._stream = stream;
            this._data = new byte[bufferSize];
            this._internalMemoryStream = new BinaryMemoryWriteStream(this._data, isLittleEndian);
            IsLittleEndian = isLittleEndian;
        }

        public BinaryWriteStream(string path, int bufferSize = 4096, bool isLittleEndian = true)
            : this(File.OpenWrite(path), bufferSize, isLittleEndian: isLittleEndian)
        {
        }

        public override void Flush()
        {
            if (_internalMemoryStream.Position == 0) return;
            var amountWritten = _internalMemoryStream.Position;
            _stream.Write(_data, 0, amountWritten);
            _internalMemoryStream.Position = 0;
            _streamFrontlinePos += amountWritten;
            _internalMemoryStream.Position = 0;
            if (_streamFrontlinePos != _stream.Position)
            {
                throw new ArgumentException();
            }
        }

        private void SetPosition(long pos)
        {
            throw new NotImplementedException();
        }

        private void LoadAmount(int amount)
        {
            if (InternalStreamRemaining < amount)
            {
                Flush();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.Flush();
            if (this._dispose)
            {
                this._stream.Dispose();
            }
        }

        public void Write(bool value)
        {
            LoadAmount(1);
            _internalMemoryStream.Write(value);
        }

        public void Write(byte value)
        {
            LoadAmount(1);
            _internalMemoryStream.Write(value);
        }

        public void Write(ushort value)
        {
            LoadAmount(2);
            _internalMemoryStream.Write(value);
        }

        public void Write(uint value)
        {
            LoadAmount(4);
            _internalMemoryStream.Write(value);
        }

        public void Write(ulong value)
        {
            LoadAmount(8);
            _internalMemoryStream.Write(value);
        }

        public void Write(sbyte value)
        {
            LoadAmount(1);
            _internalMemoryStream.Write(value);
        }

        public void Write(short value)
        {
            LoadAmount(2);
            _internalMemoryStream.Write(value);
        }

        public void Write(int value)
        {
            LoadAmount(4);
            _internalMemoryStream.Write(value);
        }

        public void Write(long value)
        {
            LoadAmount(8);
            _internalMemoryStream.Write(value);
        }

        public void Write(float value)
        {
            LoadAmount(4);
            _internalMemoryStream.Write(value);
        }

        public void Write(double value)
        {
            LoadAmount(8);
            _internalMemoryStream.Write(value);
        }

        public override void Write(byte[] buffer, int offset, int amount)
        {
            _internalMemoryStream.Write(buffer, offset, amount);
        }

        public void Write(ReadOnlySpan<char> value)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
