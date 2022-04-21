namespace Noggog;

public class ByteMemorySliceStream : Stream
{
    private ReadOnlyMemorySlice<byte> _data;
    private int _pos;

    public override bool CanRead => true;
    public override bool CanSeek => true;
    public override bool CanWrite => false;
    public override long Length => _data.Length;
    public int Remaining => _data.Length - _pos;

    public ByteMemorySliceStream(ReadOnlyMemorySlice<byte> mem)
    {
        _data = mem;
    }

    public override long Position
    {
        get => _pos;
        set => _pos = checked((int)value);
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count > Remaining)
        {
            count = Remaining;
        }
        _data.Span.Slice(_pos, count).CopyTo(buffer.AsSpan(offset));
        _pos += count;
        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                Position = offset;
                break;
            case SeekOrigin.Current:
                Position += offset;
                break;
            case SeekOrigin.End:
                Position = Length + offset;
                break;
            default:
                throw new NotImplementedException();
        }
        return Position;
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}