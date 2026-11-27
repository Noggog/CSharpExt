using Noggog;

namespace CSharpExt.UnitTests;

/// <summary>
/// Edge cases for ReadMemory / GetMemory on BinaryReadStream, focused on
/// readSafe: false (zero-copy slices into the internal buffer) around buffer boundaries.
/// </summary>
public class BinaryReadStreamReadSafeTests
{
    private const int BufferSize = 100;

    private static byte[] Bytes(int length)
    {
        var ret = new byte[length];
        for (int i = 0; i < length; i++)
        {
            ret[i] = (byte)(i % 251); // Prime modulus so values don't line up with the buffer size
        }
        return ret;
    }

    private static BinaryReadStream Reader(int length) =>
        new(new MemoryStream(Bytes(length)), bufferSize: BufferSize);

    private static void AssertContent(ReadOnlyMemorySlice<byte> mem, long streamPos, int expectedLength)
    {
        Assert.Equal(expectedLength, mem.Length);
        var span = mem.Span;
        for (int i = 0; i < expectedLength; i++)
        {
            Assert.True((byte)((streamPos + i) % 251) == span[i],
                $"Mismatch at slice index {i} (stream pos {streamPos + i}): expected {(byte)((streamPos + i) % 251)}, got {span[i]}");
        }
    }

    private static bool AliasesBuffer(BinaryReadStream reader, ReadOnlyMemorySlice<byte> mem) =>
        mem.Span.Overlaps(reader._data);

    #region ReadMemory

    [Fact]
    public void ReadMemory_Unsafe_FirstReadOnFreshStream_DoesNotAllocate()
    {
        var reader = Reader(300);
        var mem = reader.ReadMemory(10, readSafe: false);
        AssertContent(mem, 0, 10);
        Assert.Equal(10, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Fact]
    public void ReadMemory_Unsafe_ExactlyRemainingInBuffer_DoesNotAllocate()
    {
        var reader = Reader(300);
        reader.ReadMemory(60, readSafe: false);
        var mem = reader.ReadMemory(40, readSafe: false);
        AssertContent(mem, 60, 40);
        Assert.Equal(100, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Fact]
    public void ReadMemory_Unsafe_CrossesBoundary_RefillsAndSlices()
    {
        var reader = Reader(300);
        reader.ReadMemory(95, readSafe: false);
        var mem = reader.ReadMemory(10, readSafe: false);
        AssertContent(mem, 95, 10);
        Assert.Equal(105, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Fact]
    public void ReadMemory_Unsafe_FullBufferSizeFromMidBuffer()
    {
        var reader = Reader(300);
        reader.ReadMemory(95, readSafe: false);
        var mem = reader.ReadMemory(BufferSize, readSafe: false);
        AssertContent(mem, 95, BufferSize);
        Assert.Equal(195, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Fact]
    public void ReadMemory_Unsafe_LargerThanBuffer_Allocates()
    {
        var reader = Reader(300);
        reader.ReadMemory(30, readSafe: false);
        var mem = reader.ReadMemory(BufferSize + 1, readSafe: false);
        AssertContent(mem, 30, BufferSize + 1);
        Assert.Equal(131, reader.Position);
        Assert.False(AliasesBuffer(reader, mem));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    [InlineData(13)]
    [InlineData(50)]
    [InlineData(99)]
    [InlineData(100)]
    public void ReadMemory_Unsafe_SequentialChunks_WalkWholeStream(int chunk)
    {
        const int length = 1000;
        var reader = Reader(length);
        long pos = 0;
        while (pos + chunk <= length)
        {
            var mem = reader.ReadMemory(chunk, readSafe: false);
            AssertContent(mem, pos, chunk);
            pos += chunk;
            Assert.Equal(pos, reader.Position);
        }
    }

    [Fact]
    public void ReadMemory_Unsafe_MixedWithPrimitiveReads()
    {
        var reader = Reader(500);
        long pos = 0;
        for (int i = 0; i < 40; i++)
        {
            Assert.Equal((byte)(pos % 251), reader.ReadUInt8());
            pos += 1;
            reader.ReadInt32();
            pos += 4;
            var mem = reader.ReadMemory(6, readSafe: false);
            AssertContent(mem, pos, 6);
            pos += 6;
            Assert.Equal(pos, reader.Position);
        }
    }

    [Fact]
    public void ReadMemory_Unsafe_ReadsToExactEndOfStream()
    {
        var reader = Reader(250);
        reader.Position = 190;
        var mem = reader.ReadMemory(60, readSafe: false);
        AssertContent(mem, 190, 60);
        Assert.Equal(250, reader.Position);
        Assert.True(reader.Complete);
    }

    [Fact]
    public void ReadMemory_Unsafe_StreamShorterThanBuffer()
    {
        var reader = Reader(40);
        var mem = reader.ReadMemory(40, readSafe: false);
        AssertContent(mem, 0, 40);
        Assert.Equal(40, reader.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ReadMemory_PastEnd_ThrowsEndOfStream(bool readSafe)
    {
        var reader = Reader(150);
        reader.Position = 120;
        Assert.Throws<EndOfStreamException>(() => reader.ReadMemory(50, readSafe: readSafe));
    }

    [Fact]
    public void ReadMemory_Unsafe_PastEnd_OnShortStream_ThrowsEndOfStream()
    {
        var reader = Reader(50);
        Assert.Throws<EndOfStreamException>(() => reader.ReadMemory(60, readSafe: false));
    }

    [Fact]
    public void ReadMemory_Unsafe_ZeroLength()
    {
        var reader = Reader(150);
        reader.Position = 100;
        var mem = reader.ReadMemory(0, readSafe: false);
        Assert.Equal(0, mem.Length);
        Assert.Equal(100, reader.Position);
    }

    [Fact]
    public void ReadMemory_Unsafe_AfterSeekBackwardsOutOfBuffer()
    {
        var reader = Reader(500);
        reader.Position = 350;
        reader.ReadMemory(10, readSafe: false);
        reader.Position = 20;
        var mem = reader.ReadMemory(90, readSafe: false);
        AssertContent(mem, 20, 90);
        Assert.Equal(110, reader.Position);
    }

    [Fact]
    public void ReadMemory_Unsafe_AfterLargeDirectRead()
    {
        // A Read larger than the buffer bypasses it and sets forceReload
        var reader = Reader(600);
        reader.ReadMemory(10, readSafe: false);
        var big = new byte[250];
        Assert.Equal(250, reader.Read(big, 0, 250));
        Assert.Equal(260, reader.Position);
        var mem = reader.ReadMemory(20, readSafe: false);
        AssertContent(mem, 260, 20);
        Assert.Equal(280, reader.Position);
    }

    [Fact]
    public void ReadMemory_Safe_NeverAliasesBuffer()
    {
        var reader = Reader(300);
        var mem = reader.ReadMemory(10, readSafe: true);
        AssertContent(mem, 0, 10);
        Assert.False(AliasesBuffer(reader, mem));
    }

    #endregion

    #region GetMemory

    [Fact]
    public void GetMemory_Unsafe_ForwardsReadSafe_DoesNotAllocate()
    {
        var reader = Reader(300);
        reader.Position = 10;
        var mem = reader.GetMemory(20, readSafe: false);
        AssertContent(mem, 10, 20);
        Assert.Equal(10, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Fact]
    public void GetMemory_Safe_Allocates()
    {
        var reader = Reader(300);
        reader.Position = 10;
        var mem = reader.GetMemory(20, readSafe: true);
        AssertContent(mem, 10, 20);
        Assert.Equal(10, reader.Position);
        Assert.False(AliasesBuffer(reader, mem));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_CrossesBoundary(bool readSafe)
    {
        var reader = Reader(300);
        reader.Position = 95;
        var mem = reader.GetMemory(20, readSafe: readSafe);
        AssertContent(mem, 95, 20);
        Assert.Equal(95, reader.Position);
        // Subsequent reads still see correct data
        AssertContent(reader.ReadMemory(30, readSafe: false), 95, 30);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_WithOffset_WithinBuffer(bool readSafe)
    {
        var reader = Reader(300);
        reader.Position = 10;
        var mem = reader.GetMemory(5, offset: 3, readSafe: readSafe);
        AssertContent(mem, 13, 5);
        Assert.Equal(10, reader.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_WithOffset_SliceCrossesBoundary(bool readSafe)
    {
        // Seek-back after the read would land before the reloaded buffer
        var reader = Reader(300);
        reader.ReadMemory(90, readSafe: false); // Buffer holds [0, 100)
        var mem = reader.GetMemory(10, offset: 5, readSafe: readSafe);
        AssertContent(mem, 95, 10);
        Assert.Equal(90, reader.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_WithOffset_OffsetJumpsPastBuffer(bool readSafe)
    {
        // Position += offset alone would reload the buffer
        var reader = Reader(300);
        reader.ReadMemory(80, readSafe: false); // Buffer holds [0, 100)
        var mem = reader.GetMemory(10, offset: 40, readSafe: readSafe);
        AssertContent(mem, 120, 10);
        Assert.Equal(80, reader.Position);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_WithOffset_OffsetPlusAmountExceedsBufferSize(bool readSafe)
    {
        // Cannot fit offset + amount in the buffer; must fall back to a copying seek path
        var reader = Reader(400);
        reader.Position = 50;
        var mem = reader.GetMemory(60, offset: 70, readSafe: readSafe);
        AssertContent(mem, 120, 60);
        Assert.Equal(50, reader.Position);
        Assert.False(AliasesBuffer(reader, mem));
        // Reader still consistent afterwards
        AssertContent(reader.ReadMemory(10, readSafe: false), 50, 10);
    }

    [Fact]
    public void GetMemory_Unsafe_WithOffset_ExactlyBufferSize()
    {
        var reader = Reader(400);
        reader.Position = 50;
        var mem = reader.GetMemory(60, offset: 40, readSafe: false);
        AssertContent(mem, 90, 60);
        Assert.Equal(50, reader.Position);
        Assert.True(AliasesBuffer(reader, mem));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_LargerThanBuffer(bool readSafe)
    {
        var reader = Reader(400);
        reader.Position = 30;
        var mem = reader.GetMemory(BufferSize + 50, readSafe: readSafe);
        AssertContent(mem, 30, BufferSize + 50);
        Assert.Equal(30, reader.Position);
        AssertContent(reader.ReadMemory(10, readSafe: false), 30, 10);
    }

    [Fact]
    public void GetMemory_Unsafe_ToExactEndOfStream()
    {
        var reader = Reader(250);
        reader.Position = 200;
        var mem = reader.GetMemory(20, offset: 30, readSafe: false);
        AssertContent(mem, 230, 20);
        Assert.Equal(200, reader.Position);
    }

    [Fact]
    public void GetMemory_Unsafe_PastEnd_ThrowsEndOfStream()
    {
        var reader = Reader(250);
        reader.Position = 200;
        Assert.Throws<EndOfStreamException>(() => reader.GetMemory(30, offset: 30, readSafe: false));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetMemory_WithNegativeOffset(bool readSafe)
    {
        var reader = Reader(300);
        reader.Position = 150;
        var mem = reader.GetMemory(10, offset: -60, readSafe: readSafe);
        AssertContent(mem, 90, 10);
        Assert.Equal(150, reader.Position);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(4, 0)]
    [InlineData(4, 3)]
    [InlineData(24, 0)]
    [InlineData(24, 10)]
    [InlineData(17, 83)]
    public void GetMemory_Unsafe_PeekThenRead_WalkWholeStream(int peek, int offset)
    {
        // Mimics header-peek-then-consume record parsing
        const int length = 2000;
        var reader = Reader(length);
        long pos = 0;
        while (pos + offset + peek <= length && pos + 7 <= length)
        {
            var peeked = reader.GetMemory(peek, offset, readSafe: false);
            AssertContent(peeked, pos + offset, peek);
            Assert.Equal(pos, reader.Position);
            var read = reader.ReadMemory(7, readSafe: false);
            AssertContent(read, pos, 7);
            pos += 7;
        }
    }

    #endregion
}
