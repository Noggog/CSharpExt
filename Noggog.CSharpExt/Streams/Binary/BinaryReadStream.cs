namespace Noggog;

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
    public long Remaining => _length - Position;
    public bool Complete => _length <= Position;
    public override long Position { get => _streamPos - InternalStreamRemaining; set => SetPosition(value); }
    public ReadOnlySpan<byte> RemainingSpan => throw new NotImplementedException();
    public ReadOnlyMemorySlice<byte> RemainingMemory => throw new NotImplementedException();
    Stream IBinaryReadStream.BaseStream => _stream;

    public bool CheckUnderlyingStreamPosition = false;
    public bool IsPersistantBacking => false;

    public bool IsLittleEndian { get; }

    public BinaryReadStream(Stream stream, int bufferSize = 4096, bool dispose = true, bool isLittleEndian = true)
    {
        if (stream.Position != 0)
        {
            throw new NotImplementedException("Stream must start at position zero.");
        }
        _dispose = dispose;
        _stream = stream;
        _length = _stream.Length;
        _data = new byte[bufferSize];
        _internalMemoryStream = BinaryMemoryReadStream.Factory(new MemorySlice<byte>(_data), isLittleEndian: isLittleEndian);
        _internalMemoryStream.Position = _data.Length;
        _internalBufferLength = _data.Length;
        IsLittleEndian = isLittleEndian;
    }

    public BinaryReadStream(string path, int bufferSize = 4096, bool isLittleEndian = true)
        : this(File.OpenRead(path), bufferSize, isLittleEndian: isLittleEndian)
    {
    }

    private void SetPosition(long pos)
    {
        if (Position == pos) return;
        if (pos < 0)
        {
            throw new ArgumentException("Cannot move position to a negative value.");
        }
        if (pos > Length)
        {
            throw new ArgumentException("Cannot move position past the length of the stream");
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
        if (Position != pos)
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
        if (_internalMemoryStream.Complete && CompleteBuffering)
        {
            return;
        }

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
        if (amount > Remaining)
        {
            throw new DataMisalignedException($"Attempted to load {amount} bytes but only {Remaining} bytes remain in the stream.");
        }

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
            amount = _stream.Read(buffer, offset, amount);
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

    public ReadOnlySpan<byte> ReadSpan(int amount, bool readSafe = true)
    {
        // ToDo
        // Modify to not allocate if readsafe is off?
        var ret = new byte[amount];
        var read = Read(ret, offset: 0, amount: amount);
        return ret.AsSpan().Slice(0, read);
    }

    public ReadOnlySpan<byte> ReadSpan(int amount, int offset, bool readSafe = true)
    {
        if (offset == 0)
        {
            return ReadSpan(amount);
        }
        throw new NotImplementedException();
    }

    public ReadOnlySpan<byte> GetSpan(int amount, bool readSafe = true)
    {
        var ret = ReadSpan(amount);
        Position -= ret.Length;
        return ret;
    }

    public ReadOnlySpan<byte> GetSpan(int amount, int offset, bool readSafe = true)
    {
        Position += offset;
        var ret = ReadSpan(amount);
        Position -= ret.Length + offset;
        return ret;
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, bool readSafe = true)
    {
        if (!readSafe && amount < InternalStreamRemaining)
        {
            return _internalMemoryStream.ReadMemory(amount, readSafe: false);
        }
        var ret = new byte[amount];
        if (Read(ret, offset: 0, amount: amount) != amount)
        {
            throw new EndOfStreamException("Could not read desired amount");
        }
        return new MemorySlice<byte>(ret);
    }

    public ReadOnlyMemorySlice<byte> ReadMemory(int amount, int offset, bool readSafe = true)
    {
        if (offset == 0) return ReadMemory(amount, readSafe: readSafe);
        throw new NotImplementedException();
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, bool readSafe = true)
    {
        var ret = ReadMemory(amount);
        Position -= ret.Length;
        return ret;
    }

    public ReadOnlyMemorySlice<byte> GetMemory(int amount, int offset, bool readSafe = true)
    {
        Position += offset;
        var ret = ReadMemory(amount);
        Position -= ret.Length + offset;
        return ret;
    }

    public bool ReadBoolean()
    {
        LoadPosition(1);
        return _internalMemoryStream.ReadBoolean();
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

    public string ReadStringUTF8(int amount)
    {
        if (amount <= InternalStreamRemaining)
        {
            return _internalMemoryStream.ReadStringUTF8(amount);
        }
        if (amount < _data.Length)
        {
            LoadPosition();
            return _internalMemoryStream.ReadStringUTF8(amount);
        }

        forceReload = true;
        byte[] arr = new byte[amount];
        var numRead = _internalMemoryStream.Read(arr, 0, InternalStreamRemaining);

        amount -= numRead;

        _stream.Read(arr, numRead, amount);
        _streamPos += amount;
        return SpanExt.StringUTF8(arr.AsSpan().Slice(0, amount + numRead));
    }

    public int Get(byte[] buffer, int offset, int amount)
    {
        throw new NotImplementedException();
    }

    public int Get(byte[] buffer, int offset)
    {
        return Get(buffer, offset: offset, amount: buffer.Length);
    }

    public byte[] GetBytes(int amount)
    {
        var ret = new byte[amount];
        if (amount != Get(ret, offset: 0, amount: amount))
        {
            throw new IndexOutOfRangeException();
        }
        return ret;
    }

    public bool GetBoolean(int offset)
    {
        LoadPosition(1 + offset);
        return _internalMemoryStream.GetBoolean(offset);
    }

    public byte GetUInt8(int offset)
    {
        LoadPosition(1 + offset);
        return _internalMemoryStream.GetUInt8(offset);
    }

    public ushort GetUInt16(int offset)
    {
        LoadPosition(2 + offset);
        return _internalMemoryStream.GetUInt16(offset);
    }

    public uint GetUInt32(int offset)
    {
        LoadPosition(4 + offset);
        return _internalMemoryStream.GetUInt32(offset);
    }

    public ulong GetUInt64(int offset)
    {
        LoadPosition(8 + offset);
        return _internalMemoryStream.GetUInt64(offset);
    }

    public sbyte GetInt8(int offset)
    {
        LoadPosition(1 + offset);
        return _internalMemoryStream.GetInt8(offset);
    }

    public short GetInt16(int offset)
    {
        LoadPosition(2 + offset);
        return _internalMemoryStream.GetInt16(offset);
    }

    public int GetInt32(int offset)
    {
        LoadPosition(4 + offset);
        return _internalMemoryStream.GetInt32(offset);
    }

    public long GetInt64(int offset)
    {
        LoadPosition(8 + offset);
        return _internalMemoryStream.GetInt64(offset);
    }

    public float GetFloat(int offset)
    {
        LoadPosition(4 + offset);
        return _internalMemoryStream.GetFloat(offset);
    }

    public double GetDouble(int offset)
    {
        LoadPosition(8 + offset);
        return _internalMemoryStream.GetDouble(offset);
    }

    public string GetStringUTF8(int amount, int offset)
    {
        LoadPosition(amount + offset);
        return _internalMemoryStream.GetStringUTF8(amount, offset);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (_dispose)
        {
            _stream.Dispose();
        }
        _internalMemoryStream.Dispose();
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

    public void WriteTo(Stream stream, int amount)
    {
        while (amount > 0)
        {
            var toRequest = Math.Min(amount, (int)Remaining);
            if (toRequest == 0) break;  // No more data available

            LoadPosition(toRequest);
            var internalRemaining = InternalStreamRemaining;
            var toRead = amount < internalRemaining ? amount : internalRemaining;
            _internalMemoryStream.WriteTo(stream, toRead);
            amount -= toRead;
        }
    }

    public bool GetBoolean()
    {
        LoadPosition(1);
        return _internalMemoryStream.GetBoolean();
    }

    public byte GetUInt8()
    {
        LoadPosition(1);
        return _internalMemoryStream.GetUInt8();
    }

    public ushort GetUInt16()
    {
        LoadPosition(2);
        return _internalMemoryStream.GetUInt16();
    }

    public uint GetUInt32()
    {
        LoadPosition(4);
        return _internalMemoryStream.GetUInt32();
    }

    public ulong GetUInt64()
    {
        LoadPosition(8);
        return _internalMemoryStream.GetUInt64();
    }

    public sbyte GetInt8()
    {
        LoadPosition(1);
        return _internalMemoryStream.GetInt8();
    }

    public short GetInt16()
    {
        LoadPosition(2);
        return _internalMemoryStream.GetInt16();
    }

    public int GetInt32()
    {
        LoadPosition(4);
        return _internalMemoryStream.GetInt32();
    }

    public long GetInt64()
    {
        LoadPosition(8);
        return _internalMemoryStream.GetInt64();
    }

    public float GetFloat()
    {
        LoadPosition(4);
        return _internalMemoryStream.GetFloat();
    }

    public double GetDouble()
    {
        LoadPosition(8);
        return _internalMemoryStream.GetDouble();
    }

    public string GetStringUTF8(int amount)
    {
        LoadPosition(amount);
        return _internalMemoryStream.GetStringUTF8(amount);
    }
    #endregion
}