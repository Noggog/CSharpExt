using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public class BinaryWriteStream : IBinaryWriteStream
    {
        public readonly Stream _stream;
        internal readonly byte[] _data;
        internal bool _dispose;
        internal long _streamPos;
        internal readonly BinaryMemoryWriteStream _internalMemoryStream;
        private int InternalStreamRemaining => _internalMemoryStream.Remaining;

        public long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public long Length => throw new NotImplementedException();

        public BinaryWriteStream(Stream stream, int bufferSize = 4096, bool dispose = true)
        {
            this._dispose = dispose;
            this._stream = stream;
            this._data = new byte[bufferSize];
            this._internalMemoryStream = new BinaryMemoryWriteStream(this._data);
        }

        public BinaryWriteStream(string path, int bufferSize = 4096)
            : this(File.OpenWrite(path), bufferSize)
        {
        }

        public void Flush()
        {
            if (_internalMemoryStream.Position == 0) return;
            var amountWritten = _internalMemoryStream.Position;
            _stream.Write(_data, 0, amountWritten);
            _internalMemoryStream.Position = 0;
            _streamPos += amountWritten;
            _internalMemoryStream.Position = 0;
            if (_streamPos != _stream.Position)
            {
                throw new ArgumentException();
            }
        }

        private void LoadPosition(int amount)
        {
            if (InternalStreamRemaining < amount)
            {
                Flush();
            }
        }

        public void Dispose()
        {
            if (this._dispose)
            {
                this._stream.Dispose();
            }
        }

        public void WriteBool(bool value)
        {
            LoadPosition(1);
            _internalMemoryStream.WriteBool(value);
        }

        public void WriteUInt8(byte value)
        {
            LoadPosition(1);
            _internalMemoryStream.WriteUInt8(value);
        }

        public void WriteUInt16(ushort value)
        {
            LoadPosition(2);
            _internalMemoryStream.WriteUInt16(value);
        }

        public void WriteUInt32(uint value)
        {
            LoadPosition(4);
            _internalMemoryStream.WriteUInt32(value);
        }

        public void WriteUInt64(ulong value)
        {
            LoadPosition(8);
            _internalMemoryStream.WriteUInt64(value);
        }

        public void WriteInt8(sbyte value)
        {
            LoadPosition(1);
            _internalMemoryStream.WriteInt8(value);
        }

        public void WriteInt16(short value)
        {
            LoadPosition(2);
            _internalMemoryStream.WriteInt16(value);
        }

        public void WriteInt32(int value)
        {
            LoadPosition(4);
            _internalMemoryStream.WriteInt32(value);
        }

        public void WriteInt64(long value)
        {
            LoadPosition(8);
            _internalMemoryStream.WriteInt64(value);
        }

        public void WriteFloat(float value)
        {
            LoadPosition(4);
            _internalMemoryStream.WriteFloat(value);
        }

        public void WriteDouble(double value)
        {
            LoadPosition(8);
            _internalMemoryStream.WriteDouble(value);
        }

        public void Write(byte[] buffer, int offset, int amount)
        {
            if (amount <= _internalMemoryStream.Remaining)
            {

            }
        }

        public void Write(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void WriteString(string value)
        {
            throw new NotImplementedException();
        }
    }
}
