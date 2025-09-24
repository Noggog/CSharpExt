using Noggog;

namespace CSharpExt.UnitTests;

public class BinaryReadStreamTests : IBinaryStreamTests
{
    public const int BUFFER_SIZE = 100;
    public override int EdgeLocation => BUFFER_SIZE;

    public BinaryReadStream GetBinaryReadStream(int length, bool loaded = true)
    {
        var ret = new BinaryReadStream(
            new MemoryStream(GetByteArray(length)),
            bufferSize: BUFFER_SIZE);
        if (loaded)
        {
            ret.LoadPosition();
        }
        return ret; 
    }

    public override IBinaryReadStream GetStream(int length) => GetBinaryReadStream(length);

    [Fact]
    public void SetPositionToRightPastBuffer()
    {
        var reader = GetBinaryReadStream(EdgeLocation * 2);
        reader._internalMemoryStream.Position = reader._data.Length - 3;
        reader._streamPos = reader._data.Length;
        reader.Position += 4;
        Assert.Equal(EdgeLocation + 1, reader.Position);
        Assert.Equal(101, reader._data[0]);
    }

    [Fact]
    public void SetPositionWhileInLastBuffer()
    {
        var reader = GetBinaryReadStream(EdgeLocation * 2);
        reader._internalMemoryStream.Position = 0;
        reader._streamPos = reader._stream.Length;
        reader.Position += 4;
        Assert.Equal(EdgeLocation + 4, reader.Position);
    }

    #region AI Generated Tests
    [Fact]
    public void Constructor_StreamNotAtZero_ThrowsException()
    {
        var ms = new MemoryStream(GetByteArray(100));
        ms.Position = 10;
        Assert.Throws<NotImplementedException>(() => new BinaryReadStream(ms, bufferSize: BUFFER_SIZE));
    }

    [Fact]
    public void Position_SetPastLength_ThrowsException()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<ArgumentException>(() => reader.Position = 101);
    }

    [Fact]
    public void Position_SetToSameValue_NoOperation()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 50;
        var streamPos = reader._streamPos;
        var internalPos = reader._internalMemoryStream.Position;
        reader.Position = 50;
        Assert.Equal(streamPos, reader._streamPos);
        Assert.Equal(internalPos, reader._internalMemoryStream.Position);
    }

    [Fact]
    public void Position_SetWithinCurrentBuffer_UsesInternalMemoryStream()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 50;
        var streamPos = reader._streamPos;
        reader.Position = 60;
        Assert.Equal(streamPos, reader._streamPos);
        Assert.Equal(60, reader.Position);
    }

    [Fact]
    public void Position_SetBackwardWithinBuffer_UsesInternalMemoryStream()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 60;
        var streamPos = reader._streamPos;
        reader.Position = 50;
        Assert.Equal(streamPos, reader._streamPos);
        Assert.Equal(50, reader.Position);
    }

    [Fact]
    public void Position_SetBeforeCurrentBuffer_LoadsNewPosition()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 150;
        reader.Position = 10;
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void Position_SetNearForward_UsesNearnessBuffer()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 50;
        reader._internalMemoryStream.Position = reader._internalMemoryStream.Length;
        reader.Position = 55;
        Assert.Equal(55, reader.Position);
    }

    [Fact]
    public void Position_SetFarForward_SeeksStream()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 50;
        reader._internalMemoryStream.Position = reader._internalMemoryStream.Length;
        reader.Position = 150;
        Assert.Equal(150, reader.Position);
    }

    [Fact]
    public void LoadPosition_WhenInternalStreamComplete_ReadsNewData()
    {
        var reader = GetBinaryReadStream(200, loaded: false);
        reader._internalMemoryStream.Position = reader._data.Length;
        reader.LoadPosition();
        Assert.Equal(0, reader._internalMemoryStream.Position);
        Assert.Equal(BUFFER_SIZE, reader._internalBufferLength);
    }

    [Fact]
    public void LoadPosition_WhenInternalStreamNotComplete_CopiesRemainingData()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 90;
        reader.LoadPosition();
        Assert.Equal(0, reader._internalMemoryStream.Position);
        var expectedLength = 10 + (BUFFER_SIZE - 10);
        Assert.Equal(expectedLength, reader._internalBufferLength);
    }

    [Fact]
    public void Read_LargerThanBuffer_DirectStreamRead()
    {
        var reader = GetBinaryReadStream(300);
        var buffer = new byte[250];
        var read = reader.Read(buffer, 0, 250);
        Assert.Equal(250, read);
        Assert.Equal(250, reader.Position);
        for (int i = 0; i < 250; i++)
        {
            Assert.Equal(i % 256, buffer[i]);
        }
    }

    [Fact]
    public void Read_CompleteBuffering_ReturnsRemaining()
    {
        var reader = GetBinaryReadStream(50);
        reader.Position = 40;
        reader._streamPos = reader.Length;
        var buffer = new byte[20];
        var read = reader.Read(buffer, 0, 20);
        Assert.Equal(10, read);
        Assert.Equal(50, reader.Position);
    }

    [Fact]
    public void ReadBytes_ThrowsWhenNotEnoughData()
    {
        var reader = GetBinaryReadStream(50);
        Assert.Throws<IndexOutOfRangeException>(() => reader.ReadBytes(100));
    }

    [Fact]
    public void ReadBytes_ReadsCorrectAmount()
    {
        var reader = GetBinaryReadStream(100);
        var result = reader.ReadBytes(25);
        Assert.Equal(25, result.Length);
        Assert.Equal(25, reader.Position);
    }

    [Fact]
    public void ReadSpan_ReadsCorrectData()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;
        var span = reader.ReadSpan(5);
        Assert.Equal(5, span.Length);
        Assert.Equal(10, span[0]);
        Assert.Equal(15, reader.Position);
    }

    [Fact]
    public void ReadSpan_WithOffset_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => reader.ReadSpan(5, 10));
    }

    [Fact]
    public void GetSpan_DoesNotAdvancePosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;
        var span = reader.GetSpan(5);
        Assert.Equal(5, span.Length);
        Assert.Equal(10, span[0]);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void GetSpan_WithOffset_ReturnsDataAndRestoresPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;
        var span = reader.GetSpan(5, 3);
        Assert.Equal(5, span.Length);
        Assert.Equal(13, span[0]);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void ReadMemory_ReadSafeTrue_AllocatesNewArray()
    {
        var reader = GetBinaryReadStream(100);
        var memory = reader.ReadMemory(10, readSafe: true);
        Assert.Equal(10, memory.Length);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void ReadMemory_ReadSafeFalseWithinBuffer_ReturnsInternalMemory()
    {
        var reader = GetBinaryReadStream(100);
        var memory = reader.ReadMemory(10, readSafe: false);
        Assert.Equal(10, memory.Length);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void ReadMemory_ThrowsWhenNotEnoughData()
    {
        var reader = GetBinaryReadStream(50);
        Assert.Throws<EndOfStreamException>(() => reader.ReadMemory(100));
    }

    [Fact]
    public void ReadMemory_WithOffset_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => reader.ReadMemory(5, 10));
    }

    [Fact]
    public void GetMemory_DoesNotAdvancePosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;
        var memory = reader.GetMemory(5);
        Assert.Equal(5, memory.Length);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void GetMemory_WithOffset_ReturnsDataAndRestoresPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;
        var memory = reader.GetMemory(5, 3);
        Assert.Equal(5, memory.Length);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void ReadBoolean_ReadsCorrectValue()
    {
        var reader = GetBinaryReadStream(100);
        var ms = (MemoryStream)reader._stream;
        ms.Position = 1;
        ms.WriteByte(1);
        ms.Position = 0;
        reader.Position = 1;
        var result = reader.ReadBoolean();
        Assert.True(result);
        Assert.Equal(2, reader.Position);
    }

    [Fact]
    public void ReadByte_OverriddenMethod_ReturnsInt()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 5;
        var result = reader.ReadByte();
        Assert.Equal(5, result);
        Assert.Equal(6, reader.Position);
    }

    [Fact]
    public void ReadDouble_LoadsPositionIfNeeded()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = BUFFER_SIZE - 2;
        var result = reader.ReadDouble();
        Assert.Equal(BUFFER_SIZE + 6, reader.Position);
    }

    [Fact]
    public void ReadStringUTF8_WithinBuffer_ReadsFromInternal()
    {
        var reader = GetBinaryReadStream(100);
        var result = reader.ReadStringUTF8(5);
        Assert.Equal(5, result.Length);
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void ReadStringUTF8_SmallerThanBufferButNeedsLoad_LoadsFirst()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = BUFFER_SIZE - 2;
        var result = reader.ReadStringUTF8(10);
        Assert.Equal(10, result.Length);
        Assert.Equal(BUFFER_SIZE + 8, reader.Position);
    }

    [Fact]
    public void ReadStringUTF8_LargerThanBuffer_DirectRead()
    {
        var reader = GetBinaryReadStream(300);
        reader.Position = 50;
        var result = reader.ReadStringUTF8(150);
        Assert.Equal(150, result.Length);
        Assert.Equal(200, reader.Position);
    }

    [Fact]
    public void Get_WithOffset_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        var buffer = new byte[10];
        Assert.Throws<NotImplementedException>(() => reader.Get(buffer, 0, 10));
    }

    [Fact]
    public void GetBytes_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(50);
        Assert.Throws<NotImplementedException>(() => reader.GetBytes(100));
    }

    [Fact]
    public void GetBoolean_WithOffset_LoadsPosition()
    {
        var reader = GetBinaryReadStream(100);
        var ms = (MemoryStream)reader._stream;
        ms.Position = 10;
        ms.WriteByte(1);
        ms.Position = 0;
        reader.Position = 5;
        var result = reader.GetBoolean(5);
        Assert.True(result);
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void GetUInt16_WithOffset_LoadsPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 5;
        var result = reader.GetUInt16(3);
        Assert.Equal(2312, result);
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void GetInt32_WithOffset_LoadsPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 5;
        var result = reader.GetInt32(3);
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void GetStringUTF8_WithOffset_LoadsPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 5;
        var result = reader.GetStringUTF8(3, 2);
        Assert.Equal(3, result.Length);
        Assert.Equal(5, reader.Position);
    }

    [Fact]
    public void Seek_FromBegin_SetsPosition()
    {
        var reader = GetBinaryReadStream(100);
        var pos = reader.Seek(50, SeekOrigin.Begin);
        Assert.Equal(50, pos);
        Assert.Equal(50, reader.Position);
    }

    [Fact]
    public void Seek_FromCurrent_AddsToPosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 30;
        var pos = reader.Seek(20, SeekOrigin.Current);
        Assert.Equal(50, pos);
        Assert.Equal(50, reader.Position);
    }

    [Fact]
    public void Seek_FromEnd_OffsetsFromLength()
    {
        var reader = GetBinaryReadStream(100);
        var pos = reader.Seek(-10, SeekOrigin.End);
        Assert.Equal(90, pos);
        Assert.Equal(90, reader.Position);
    }

    [Fact]
    public void Flush_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => reader.Flush());
    }

    [Fact]
    public void SetLength_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => reader.SetLength(50));
    }

    [Fact]
    public void Write_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => reader.Write(new byte[10], 0, 10));
    }

    [Fact]
    public void WriteTo_CopiesDataToStream()
    {
        var reader = GetBinaryReadStream(100);
        var output = new MemoryStream();
        reader.WriteTo(output, 25);
        Assert.Equal(25, output.Length);
        Assert.Equal(25, reader.Position);
    }

    [Fact]
    public void WriteTo_AcrossBuffers_CopiesCorrectly()
    {
        var reader = GetBinaryReadStream(200);
        var output = new MemoryStream();
        reader.WriteTo(output, 150);
        Assert.Equal(150, output.Length);
        Assert.Equal(150, reader.Position);
    }

    [Fact]
    public void Dispose_DisposesStreamWhenDisposeTrue()
    {
        var ms = new MemoryStream(GetByteArray(100));
        var reader = new BinaryReadStream(ms, bufferSize: BUFFER_SIZE, dispose: true);
        reader.Dispose();
        Assert.Throws<ObjectDisposedException>(() => ms.Position);
    }

    [Fact]
    public void Dispose_DoesNotDisposeStreamWhenDisposeFalse()
    {
        var ms = new MemoryStream(GetByteArray(100));
        var reader = new BinaryReadStream(ms, bufferSize: BUFFER_SIZE, dispose: false);
        reader.Dispose();
        Assert.Equal(0, ms.Position);
    }

    [Fact]
    public void GetNoOffset_Methods_DoNotAdvancePosition()
    {
        var reader = GetBinaryReadStream(100);
        reader.Position = 10;

        reader.GetBoolean();
        Assert.Equal(10, reader.Position);

        reader.GetUInt8();
        Assert.Equal(10, reader.Position);

        reader.GetUInt16();
        Assert.Equal(10, reader.Position);

        reader.GetUInt32();
        Assert.Equal(10, reader.Position);

        reader.GetUInt64();
        Assert.Equal(10, reader.Position);

        reader.GetInt8();
        Assert.Equal(10, reader.Position);

        reader.GetInt16();
        Assert.Equal(10, reader.Position);

        reader.GetInt32();
        Assert.Equal(10, reader.Position);

        reader.GetInt64();
        Assert.Equal(10, reader.Position);

        reader.GetFloat();
        Assert.Equal(10, reader.Position);

        reader.GetDouble();
        Assert.Equal(10, reader.Position);

        reader.GetStringUTF8(5);
        Assert.Equal(10, reader.Position);
    }

    [Fact]
    public void StreamProperties_ReturnCorrectValues()
    {
        var reader = GetBinaryReadStream(100);
        Assert.True(reader.CanRead);
        Assert.True(reader.CanSeek);
        Assert.False(reader.CanWrite);
    }

    [Fact]
    public void IsLittleEndian_DefaultsToTrue()
    {
        var reader = GetBinaryReadStream(100);
        Assert.True(reader.IsLittleEndian);
    }

    [Fact]
    public void IsLittleEndian_CanBeSetToFalse()
    {
        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(100)),
            bufferSize: BUFFER_SIZE,
            isLittleEndian: false);
        Assert.False(reader.IsLittleEndian);
    }

    [Fact]
    public void IsPersistantBacking_AlwaysFalse()
    {
        var reader = GetBinaryReadStream(100);
        Assert.False(reader.IsPersistantBacking);
    }

    [Fact]
    public void RemainingSpan_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => _ = reader.RemainingSpan);
    }

    [Fact]
    public void RemainingMemory_ThrowsNotImplemented()
    {
        var reader = GetBinaryReadStream(100);
        Assert.Throws<NotImplementedException>(() => _ = reader.RemainingMemory);
    }

    [Fact]
    public void CompleteBuffering_TrueWhenStreamFullyRead()
    {
        var reader = GetBinaryReadStream(50);
        reader.Position = 50;
        Assert.True(reader.CompleteBuffering);
    }

    [Fact]
    public void CompleteBuffering_FalseWhenMoreDataAvailable()
    {
        var reader = GetBinaryReadStream(200);
        reader.Position = 50;
        Assert.False(reader.CompleteBuffering);
    }
    #endregion
}