using System.IO;

namespace Noggog.Testing.IO
{
    public class LengthLiarStream : Stream
    {
        private readonly Stream _wrap;
        private readonly int _lengthOverride;

        public LengthLiarStream(Stream wrap, int lengthOverride)
        {
            _wrap = wrap;
            _lengthOverride = lengthOverride;
        }

        public override void Flush()
        {
            _wrap.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _wrap.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _wrap.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _wrap.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _wrap.Write(buffer, offset, count);
        }

        public override bool CanRead => _wrap.CanRead;

        public override bool CanSeek => _wrap.CanSeek;

        public override bool CanWrite => _wrap.CanWrite;

        public override long Length => _lengthOverride;

        public override long Position
        {
            get => _wrap.Position;
            set => _wrap.Position = value;
        }
    }
}