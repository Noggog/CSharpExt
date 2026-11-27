using System.Buffers.Binary;
using Noggog;

namespace CSharpExt.UnitTests;

/// <summary>
/// Regression tests for BinaryReadStream state/boundary bugs.
/// Ground truth is always the source byte array.
/// </summary>
public class BinaryReadStreamRobustnessTests
{
    private static byte[] Bytes(int length)
    {
        var ret = new byte[length];
        for (int i = 0; i < length; i++)
        {
            ret[i] = (byte)(i % 251);
        }
        return ret;
    }

    private static byte At(long pos) => (byte)(pos % 251);

    private static BinaryReadStream Reader(int length, int bufferSize) =>
        new(new MemoryStream(Bytes(length)), bufferSize: bufferSize);

    #region Stale forceReload

    [Fact]
    public void SeekToStreamPos_AfterLargeRead_ThenSmallRead()
    {
        var r = Reader(100, bufferSize: 10);
        Assert.Equal(50, r.Read(new byte[50], 0, 50)); // Bypasses buffer, sets forceReload
        Assert.Equal(At(50), r.ReadUInt8());           // Loads [50, 60)
        r.Position = 60;                               // Exactly _streamPos
        Assert.Equal(60, r.Position);
        Assert.Equal(At(60), r.ReadUInt8());
    }

    [Fact]
    public void SeekToEnd_AfterLargeRead()
    {
        var r = Reader(299, bufferSize: 100);
        r.ReadMemory(271);
        Assert.Equal(At(271), r.GetUInt8());
        r.Seek(0, SeekOrigin.End);
        Assert.Equal(299, r.Position);
        Assert.True(r.Complete);
    }

    [Fact]
    public void InBufferSeeks_AfterLargeReadThenReload_ReturnCorrectData()
    {
        var r = Reader(300, bufferSize: 16);
        r.ReadBytes(40);                 // Large read
        Assert.Equal(At(40), r.ReadUInt8());
        r.Position = 45;
        Assert.Equal(At(45), r.ReadUInt8());
        r.Position = 41;
        Assert.Equal(At(41), r.ReadUInt8());
        r.Position = 56;                 // == _streamPos
        Assert.Equal(At(56), r.ReadUInt8());
    }

    [Fact]
    public void SeekToStreamPos_AfterLargeReadStringUTF8()
    {
        var r = Reader(172, bufferSize: 16);
        r.ReadBytes(10);
        r.ReadStringUTF8(20);            // >= buffer: direct path, sets forceReload
        r.ReadStringUTF8(13);
        r.ReadBoolean();
        r.GetDouble();
        r.Position = 60;
        Assert.Equal(60, r.Position);
        Assert.Equal(At(60), r.ReadUInt8());
    }

    #endregion

    #region ReadStringUTF8 past end

    [Fact]
    public void ReadStringUTF8_SmallPastEnd_AfterPartialFinalBuffer_Throws()
    {
        // 15 bytes, buffer 10: final fill is short, leaving stale bytes past the valid region
        var r = Reader(15, bufferSize: 10);
        r.ReadUInt64();
        r.ReadUInt32();
        Assert.Equal(12, r.Position);
        Assert.Throws<EndOfStreamException>(() => r.ReadStringUTF8(5));
        Assert.Equal(12, r.Position);
        // Reader still consistent afterwards
        var s = r.ReadStringUTF8(3);
        Assert.Equal(new string(new[] { (char)At(12), (char)At(13), (char)At(14) }), s);
        r.Position = 0;
        Assert.Equal(At(0), r.ReadUInt8());
    }

    [Fact]
    public void ReadStringUTF8_SmallPastEnd_Throws()
    {
        var r = Reader(20, bufferSize: 16);
        r.ReadBytes(12);
        Assert.Throws<EndOfStreamException>(() => r.ReadStringUTF8(10));
        Assert.Equal(12, r.Position);
    }

    [Fact]
    public void ReadStringUTF8_AtEnd_Throws()
    {
        var r = Reader(19, bufferSize: 100);
        r.Position = 19;
        Assert.Throws<EndOfStreamException>(() => r.ReadStringUTF8(2));
        Assert.Equal(19, r.Position);
        r.Position = 0;
        Assert.Equal(At(9), r.GetUInt8(9));
    }

    [Theory]
    [InlineData(30, 40)]
    [InlineData(144, 145)]
    public void ReadStringUTF8_LargePastEnd_Throws(int length, int amount)
    {
        var r = Reader(length, bufferSize: 10);
        Assert.Throws<EndOfStreamException>(() => r.ReadStringUTF8(amount));
        Assert.Equal(0, r.Position);
        Assert.Equal(At(0), r.ReadUInt8());
    }

    [Fact]
    public void ReadStringUTF8_LargePastEnd_MidStream_Throws()
    {
        var r = Reader(40, bufferSize: 16);
        r.ReadBytes(30);
        Assert.Throws<EndOfStreamException>(() => r.ReadStringUTF8(20));
        Assert.Equal(30, r.Position);
        Assert.True(r.Position <= r.Length);
    }

    [Theory]
    [InlineData(8, 8)]   // Fits after refill
    [InlineData(0, 16)]  // Exactly buffer size
    [InlineData(5, 15)]  // Buffer-1, crosses boundary
    [InlineData(3, 37)]  // Larger than buffer, ends exactly at stream end
    [InlineData(30, 10)] // Small, ends exactly at stream end
    public void ReadStringUTF8_InRange_ReturnsCorrectText(int start, int amount)
    {
        var r = Reader(40, bufferSize: 16);
        r.ReadBytes(start);
        var pos = r.Position;
        var s = r.ReadStringUTF8(amount);
        Assert.Equal(amount, s.Length);
        for (int i = 0; i < amount; i++)
        {
            Assert.Equal((char)At(pos + i), s[i]);
        }
        Assert.Equal(pos + amount, r.Position);
    }

    #endregion

    #region Short reads from underlying stream

    /// <summary>
    /// Returns at most maxPerRead bytes per Read call, which Stream's contract allows
    /// (network, pipe, decompression streams, user wrappers).
    /// </summary>
    private class ChunkedStream : MemoryStream
    {
        private readonly int _maxPerRead;

        public ChunkedStream(byte[] data, int maxPerRead) : base(data)
        {
            _maxPerRead = maxPerRead;
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            base.Read(buffer, offset, Math.Min(count, _maxPerRead));

        public override int Read(Span<byte> buffer) =>
            base.Read(buffer.Slice(0, Math.Min(buffer.Length, _maxPerRead)));
    }

    private static BinaryReadStream ChunkedReader(int length, int bufferSize, int maxPerRead) =>
        new(new ChunkedStream(Bytes(length), maxPerRead), bufferSize: bufferSize);

    public static IEnumerable<object[]> ChunkCases()
    {
        foreach (var buf in new[] { 16, 100 })
        {
            foreach (var chunk in new[] { 1, 3, 7, 30 })
            {
                yield return new object[] { buf, chunk };
            }
        }
    }

    private static void AssertBytes(ReadOnlySpan<byte> actual, long pos, int length)
    {
        Assert.Equal(length, actual.Length);
        for (int i = 0; i < length; i++)
        {
            Assert.True(At(pos + i) == actual[i], $"Mismatch at stream pos {pos + i}: expected {At(pos + i)}, got {actual[i]}");
        }
    }

    [Theory]
    [MemberData(nameof(ChunkCases))]
    public void ShortReads_Primitives(int bufferSize, int chunk)
    {
        var r = ChunkedReader(500, bufferSize, chunk);
        long pos = 0;
        while (pos + 15 <= 500)
        {
            Assert.Equal(At(pos), r.ReadUInt8());
            Assert.Equal(BitConverter.ToInt32(Bytes(500), (int)pos + 1), r.ReadInt32());
            Assert.Equal(BitConverter.ToInt64(Bytes(500), (int)pos + 5), r.GetInt64());
            Assert.Equal(BitConverter.ToUInt16(Bytes(500), (int)pos + 13), r.GetUInt16(8));
            r.ReadInt64();
            pos += 13;
            Assert.Equal(pos, r.Position);
        }
    }

    [Theory]
    [MemberData(nameof(ChunkCases))]
    public void ShortReads_BulkReads(int bufferSize, int chunk)
    {
        var r = ChunkedReader(4000, bufferSize, chunk);
        long pos = 0;
        foreach (var amount in new[] { 10, 5, bufferSize - 1, bufferSize, bufferSize + 1, 2 * bufferSize + 3, 7 })
        {
            AssertBytes(r.ReadBytes(amount), pos, amount);
            pos += amount;
            AssertBytes(r.ReadMemory(amount, readSafe: false), pos, amount);
            pos += amount;
            AssertBytes(r.ReadMemory(amount, readSafe: true), pos, amount);
            pos += amount;
            var s = r.ReadStringUTF8(amount);
            Assert.Equal(System.Text.Encoding.UTF8.GetString(Bytes(4000), (int)pos, amount), s);
            pos += amount;
            var ms = new MemoryStream();
            r.WriteTo(ms, amount);
            AssertBytes(ms.ToArray(), pos, amount);
            pos += amount;
            Assert.Equal(pos, r.Position);
        }
    }

    [Theory]
    [MemberData(nameof(ChunkCases))]
    public void ShortReads_Seeks(int bufferSize, int chunk)
    {
        var r = ChunkedReader(1000, bufferSize, chunk);
        r.CheckUnderlyingStreamPosition = true;
        foreach (var target in new long[] { 5, 17, 30, 12, 200, 205, 219, 600, 3, 999, 0 })
        {
            r.Position = target;
            Assert.Equal(target, r.Position);
            Assert.Equal(At(target), r.GetUInt8());
            if (target + 10 < 1000)
            {
                r.Position += 10; // Small forward skips use the "nearness" read path
                Assert.Equal(At(target + 10), r.GetUInt8());
            }
        }
    }

    #endregion

    #region Peeks larger than the buffer

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(40)]
    [InlineData(150)]
    public void GetPrimitivesWithOffset_BeyondBuffer(int offset)
    {
        var data = Bytes(400);
        var r = Reader(400, bufferSize: 16);
        r.ReadBytes(7);
        var p = 7 + offset;
        Assert.Equal(data[p], r.GetUInt8(offset));
        Assert.Equal((sbyte)data[p], r.GetInt8(offset));
        Assert.Equal(data[p] > 0, r.GetBoolean(offset));
        Assert.Equal(BitConverter.ToUInt16(data, p), r.GetUInt16(offset));
        Assert.Equal(BitConverter.ToInt16(data, p), r.GetInt16(offset));
        Assert.Equal(BitConverter.ToUInt32(data, p), r.GetUInt32(offset));
        Assert.Equal(BitConverter.ToInt32(data, p), r.GetInt32(offset));
        Assert.Equal(BitConverter.ToUInt64(data, p), r.GetUInt64(offset));
        Assert.Equal(BitConverter.ToInt64(data, p), r.GetInt64(offset));
        Assert.Equal(BitConverter.ToSingle(data, p), r.GetFloat(offset));
        Assert.Equal(BitConverter.ToDouble(data, p), r.GetDouble(offset));
        Assert.Equal(7, r.Position);
        Assert.Equal(data[7], r.ReadUInt8());
    }

    [Fact]
    public void GetPrimitivesWithNegativeOffset()
    {
        var data = Bytes(400);
        var r = Reader(400, bufferSize: 16);
        r.ReadBytes(100);
        Assert.Equal(BitConverter.ToInt32(data, 40), r.GetInt32(-60));
        Assert.Equal(data[99], r.GetUInt8(-1));
        Assert.Equal(100, r.Position);
        Assert.Equal(data[100], r.ReadUInt8());
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(16, 0)]
    [InlineData(40, 0)]
    [InlineData(10, 20)]
    [InlineData(30, 30)]
    public void GetStringUTF8_BeyondBuffer(int amount, int offset)
    {
        var data = Bytes(120);
        var r = Reader(120, bufferSize: 16);
        r.ReadBytes(3);
        var expected = System.Text.Encoding.UTF8.GetString(data, 3 + offset, amount);
        Assert.Equal(expected, r.GetStringUTF8(amount, offset));
        if (offset == 0)
        {
            Assert.Equal(expected, r.GetStringUTF8(amount));
        }
        Assert.Equal(3, r.Position);
    }

    [Fact]
    public void GetWithOffset_BeyondBuffer_PastEnd_Throws_PositionUnchanged()
    {
        var r = Reader(100, bufferSize: 16);
        r.ReadBytes(10);
        Assert.Throws<EndOfStreamException>(() => r.GetInt32(87));
        Assert.Throws<EndOfStreamException>(() => r.GetStringUTF8(50, 45));
        Assert.Equal(10, r.Position);
        Assert.Equal(At(10), r.ReadUInt8());
    }

    [Fact]
    public void GetInt32_DefaultBuffer_OffsetNearBufferEnd()
    {
        var data = Bytes(10000);
        var r = new BinaryReadStream(new MemoryStream(data));
        Assert.Equal(BitConverter.ToInt32(data, 4094), r.GetInt32(4094));
        Assert.Equal(BitConverter.ToInt32(data, 5000), r.GetInt32(5000));
        Assert.Equal(0, r.Position);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(7)]
    public void Ctor_BufferTooSmall_Throws(int bufferSize)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BinaryReadStream(new MemoryStream(Bytes(10)), bufferSize: bufferSize));
    }

    [Fact]
    public void MinimumBuffer_ReadsEightByteValues()
    {
        var data = Bytes(100);
        var r = Reader(100, bufferSize: 8);
        r.ReadBytes(3);
        Assert.Equal(BitConverter.ToInt64(data, 3), r.ReadInt64());
        Assert.Equal(BitConverter.ToDouble(data, 11), r.ReadDouble());
        Assert.Equal(BitConverter.ToInt64(data, 30), r.GetInt64(11));
    }

    #endregion

    #region Past end of stream: consistent EndOfStreamException, Position unchanged

    public static IEnumerable<object[]> PastEndOps()
    {
        // name, needed bytes beyond current position (op is invoked with remaining = 5)
        var ops = new (string, Action<BinaryReadStream>)[]
        {
            ("ReadBytes", r => r.ReadBytes(6)),
            ("ReadSpan", r => r.ReadSpan(6)),
            ("GetSpan", r => r.GetSpan(6)),
            ("GetSpanOffset", r => r.GetSpan(3, offset: 3)),
            ("WriteTo", r => r.WriteTo(new MemoryStream(), 6)),
            ("WriteToLarge", r => r.WriteTo(new MemoryStream(), 100)),
            ("ReadInt64", r => r.ReadInt64()),
            ("ReadDouble", r => r.ReadDouble()),
            ("GetInt64", r => r.GetInt64()),
            ("GetInt32Offset", r => r.GetInt32(2)),
            ("GetInt32OffsetBeyondBuffer", r => r.GetInt32(40)),
            ("ReadStringUTF8", r => r.ReadStringUTF8(6)),
            ("ReadStringUTF8Large", r => r.ReadStringUTF8(40)),
            ("GetStringUTF8", r => r.GetStringUTF8(6)),
            ("GetStringUTF8Offset", r => r.GetStringUTF8(3, 3)),
            ("GetStringUTF8Large", r => r.GetStringUTF8(40)),
        };
        foreach (var (name, op) in ops)
        {
            yield return new object[] { name, op };
        }
    }

    [Theory]
    [MemberData(nameof(PastEndOps))]
    public void PastEnd_ThrowsEndOfStream_PositionUnchanged(string name, Action<BinaryReadStream> op)
    {
        _ = name;
        var r = Reader(60, bufferSize: 16);
        r.ReadBytes(55);
        Assert.Throws<EndOfStreamException>(() => op(r));
        Assert.Equal(55, r.Position);
        Assert.Equal(At(55), r.ReadUInt8());
    }

    [Fact]
    public void WriteTo_ExactlyRemaining_Succeeds()
    {
        var r = Reader(60, bufferSize: 16);
        r.ReadBytes(5);
        var ms = new MemoryStream();
        r.WriteTo(ms, 55);
        Assert.Equal(Bytes(60).AsSpan(5).ToArray(), ms.ToArray());
        Assert.True(r.Complete);
    }

    #endregion

    #region ReadByte at end of stream

    [Fact]
    public void ReadByte_AtEnd_ReturnsMinusOne()
    {
        var r = Reader(20, bufferSize: 16);
        var read = new List<int>();
        int b;
        while ((b = r.ReadByte()) != -1)
        {
            read.Add(b);
        }
        Assert.Equal(Bytes(20).Select(x => (int)x), read);
        Assert.Equal(-1, r.ReadByte());
        Assert.Equal(20, r.Position);
    }

    [Fact]
    public void ReadByte_EmptyStream_ReturnsMinusOne()
    {
        Assert.Equal(-1, Reader(0, bufferSize: 16).ReadByte());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MemoryReadStream_ReadByte_AtEnd_ReturnsMinusOne(bool littleEndian)
    {
        var r = BinaryMemoryReadStream.Factory(Bytes(3), littleEndian);
        Assert.Equal(At(0), r.ReadByte());
        Assert.Equal(At(1), r.ReadByte());
        Assert.Equal(At(2), r.ReadByte());
        Assert.Equal(-1, r.ReadByte());
        Assert.Equal(3, r.Position);
    }

    #endregion

    #region Read exactly the buffered remainder

    private class CountingStream : MemoryStream
    {
        public int ReadCalls;
        public CountingStream(byte[] data) : base(data) { }
        public override int Read(byte[] buffer, int offset, int count)
        {
            ReadCalls++;
            return base.Read(buffer, offset, count);
        }
    }

    [Fact]
    public void Read_ExactlyBufferedRemainder_DoesNotRefill()
    {
        var counting = new CountingStream(Bytes(100));
        var r = new BinaryReadStream(counting, bufferSize: 16);
        var buf = new byte[16];
        Assert.Equal(16, r.Read(buf, 0, 16));
        Assert.Equal(Bytes(16), buf);
        Assert.Equal(1, counting.ReadCalls);

        // Read the rest of the second buffer exactly
        r.ReadUInt32();
        Assert.Equal(2, counting.ReadCalls);
        Assert.Equal(12, r.Read(buf, 0, 12));
        Assert.Equal(2, counting.ReadCalls);
        Assert.Equal(Bytes(32).AsSpan(20, 12).ToArray(), buf.AsSpan(0, 12).ToArray());
        Assert.Equal(32, r.Position);
    }

    [Fact]
    public void Read_ZeroBytes_DoesNotRefill()
    {
        var counting = new CountingStream(Bytes(100));
        var r = new BinaryReadStream(counting, bufferSize: 16);
        Assert.Equal(0, r.Read(new byte[1], 0, 0));
        Assert.Equal(0, counting.ReadCalls);
        Assert.Equal(0, r.Position);
    }

    #endregion

    #region BinaryMemoryReadStream: failed reads must not move Position

    public static IEnumerable<object[]> MemoryStreamFailingReads()
    {
        var ops = new (string, Action<BinaryMemoryReadStream>)[]
        {
            ("ReadUInt16", r => r.ReadUInt16()),
            ("ReadInt16", r => r.ReadInt16()),
            ("ReadUInt32", r => r.ReadUInt32()),
            ("ReadInt32", r => r.ReadInt32()),
            ("ReadUInt64", r => r.ReadUInt64()),
            ("ReadInt64", r => r.ReadInt64()),
            ("ReadFloat", r => r.ReadFloat()),
            ("ReadDouble", r => r.ReadDouble()),
            ("ReadSpan", r => r.ReadSpan(5)),
            ("ReadSpanOffset", r => r.ReadSpan(1, offset: 4)),
            ("ReadMemory", r => r.ReadMemory(5)),
            ("ReadMemoryOffset", r => r.ReadMemory(1, offset: 4)),
            ("ReadStringUTF8", r => r.ReadStringUTF8(5)),
            ("ReadBytes", r => r.ReadBytes(5)),
        };
        foreach (var little in new[] { true, false })
        foreach (var (name, op) in ops)
        {
            yield return new object[] { name, little, op };
        }
    }

    [Theory]
    [MemberData(nameof(MemoryStreamFailingReads))]
    public void MemoryReadStream_FailedRead_PositionUnchanged(string name, bool littleEndian, Action<BinaryMemoryReadStream> op)
    {
        _ = name;
        var r = BinaryMemoryReadStream.Factory(Bytes(4), littleEndian);
        r.Position = 3; // 1 byte remaining
        Assert.ThrowsAny<Exception>(() => op(r));
        Assert.Equal(3, r.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MemoryReadStream_SingleByteReadsAtEnd_PositionUnchanged(bool littleEndian)
    {
        var r = BinaryMemoryReadStream.Factory(Bytes(2), littleEndian);
        r.Position = 2;
        Assert.ThrowsAny<Exception>(() => r.ReadUInt8());
        Assert.ThrowsAny<Exception>(() => r.ReadInt8());
        Assert.ThrowsAny<Exception>(() => r.ReadBoolean());
        Assert.Equal(2, r.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void MemoryReadStream_SuccessfulReads_StillAdvance(bool littleEndian)
    {
        var data = Bytes(64);
        var r = BinaryMemoryReadStream.Factory(data, littleEndian);
        Assert.Equal(data[0], r.ReadUInt8());
        Assert.Equal((sbyte)data[1], r.ReadInt8());
        Assert.Equal(data[2] > 0, r.ReadBoolean());
        var u16 = r.ReadUInt16();
        Assert.Equal(littleEndian ? BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(3)) : BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(3)), u16);
        var i64 = r.ReadInt64();
        Assert.Equal(littleEndian ? BinaryPrimitives.ReadInt64LittleEndian(data.AsSpan(5)) : BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(5)), i64);
        var f = r.ReadFloat();
        Assert.Equal(littleEndian ? BinaryPrimitives.ReadSingleLittleEndian(data.AsSpan(13)) : BinaryPrimitives.ReadSingleBigEndian(data.AsSpan(13)), f);
        var d = r.ReadDouble();
        Assert.Equal(littleEndian ? BinaryPrimitives.ReadDoubleLittleEndian(data.AsSpan(17)) : BinaryPrimitives.ReadDoubleBigEndian(data.AsSpan(17)), d);
        Assert.Equal(data.AsSpan(25, 3).ToArray(), r.ReadSpan(3).ToArray());
        Assert.Equal(data.AsSpan(30, 2).ToArray(), r.ReadSpan(2, offset: 2).ToArray());
        Assert.Equal(32, r.Position);
        Assert.Equal(data.AsSpan(34, 2).ToArray(), r.ReadMemory(2, offset: 2).ToArray());
        Assert.Equal(36, r.Position);
        Assert.Equal(System.Text.Encoding.UTF8.GetString(data, 36, 4), r.ReadStringUTF8(4));
        Assert.Equal(40, r.Position);
    }

    [Fact]
    public void MemoryReadStream_Write_NotSupported()
    {
        var r = BinaryMemoryReadStream.Factory(Bytes(10), isLittleEndian: true);
        var buf = new byte[4];
        Assert.False(r.CanWrite);
        Assert.Throws<NotSupportedException>(() => r.Write(buf, 0, 4));
        Assert.Equal(new byte[4], buf);
    }

    #endregion
}
