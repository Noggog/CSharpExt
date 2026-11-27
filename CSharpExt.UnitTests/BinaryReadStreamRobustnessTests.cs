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
}
