using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog.Streams.Binary
{
    public class BasicSubstitutionRangeStream : Stream
    {
        private Stream sourceStream;
        private RangeCollection subList;
        private byte toSubstitute;
        
        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => false;
        public override long Length => sourceStream.Length;
        public override long Position
        {
            get => sourceStream.Position;
            set => sourceStream.Position = value;
        }

        public BasicSubstitutionRangeStream(
            Stream sourceStream,
            RangeCollection subList,
            byte toSubstitute)
        {
            this.toSubstitute = toSubstitute;
            this.sourceStream = sourceStream;
            this.subList = subList;
        }

        public override void Flush()
        {
            sourceStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = (int)Math.Min(count, sourceStream.Remaining());
            int amountRead = 0;
            while (count > 0)
            {
                var pos = sourceStream.Position;
                if (subList.TryGetCurrentOrNextRange(pos, out var range))
                {
                    int diff = (int)Math.Min(count, sourceStream.Length - sourceStream.Position);
                    if (range.IsInRange(pos))
                    {
                        diff = Math.Min(diff, (int)(range.Max - pos + 1));
                        for (int i = 0; i < diff; i++)
                        {
                            buffer[i + offset] = toSubstitute;
                        }
                        sourceStream.Position += diff;
                    }
                    else
                    {
                        diff = Math.Min(diff, (int)(range.Min - pos));
                        sourceStream.Read(buffer, offset, diff);
                    }
                    offset += diff;
                    count -= diff;
                    amountRead += diff;
                }
                else
                {
                    amountRead += sourceStream.Read(buffer, offset, count);
                    break;
                }
            }
            return amountRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return sourceStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            sourceStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
