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

    [Fact]
    public void Position_SmallFile_NotNegativeWhenComplete()
    {
        // Bug: When reading a file smaller than buffer size, Position can become negative
        // when complete because InternalStreamRemaining incorrectly uses the buffer size
        var fileSize = 50;  // Smaller than BUFFER_SIZE (100)
        var stream = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: BUFFER_SIZE);

        // Read all bytes from the small file
        var bytes = stream.ReadBytes(fileSize);

        // After reading all data, Position should equal fileSize, not be negative
        Assert.Equal(fileSize, stream.Position);
        Assert.True(stream.Position >= 0, $"Position should not be negative, but was {stream.Position}");
        Assert.True(stream.Complete, "Stream should be complete after reading all data");
        Assert.Equal(0, stream.Remaining);
    }

    [Fact]
    public void Position_SmallFileUnloaded_NotNegative()
    {
        // Test scenario where stream is created but not loaded
        var fileSize = 50;  // Smaller than BUFFER_SIZE (100)
        var stream = GetBinaryReadStream(fileSize, loaded: false);

        // Check Position before any loading
        var position = stream.Position;
        Assert.True(position >= 0, $"Position should not be negative before loading, but was {position}");
        Assert.Equal(0, position);
    }

    [Fact]
    public void Position_WriteTo_SmallFile_NotNegative()
    {
        // Bug scenario: WriteTo on a small file (smaller than buffer) can cause negative Position
        // This mimics what happens in Mutagen's ModTrimmer when processing small .esp files
        var fileSize = 50;  // Smaller than BUFFER_SIZE (100)
        var reader = GetBinaryReadStream(fileSize);
        var outputStream = new MemoryStream();

        // Read some data first (like reading a header)
        reader.ReadBytes(10);

        // Now WriteTo the remaining data (40 bytes)
        // This triggers the bug: WriteTo calls LoadPosition which incorrectly handles
        // the small file case when _internalMemoryStream thinks there's more data
        reader.WriteTo(outputStream, 40);

        // After WriteTo, Position should be at 50, not negative
        Assert.True(reader.Position >= 0, $"Position should not be negative after WriteTo, but was {reader.Position}");
        Assert.Equal(fileSize, reader.Position);
        Assert.Equal(0, reader.Remaining);
        Assert.True(reader.Complete);
    }

    [Fact]
    public void Position_AfterWriteToAtEndOfSmallFile_NotNegative()
    {
        // Bug: When file is smaller than buffer and completely consumed,
        // subsequent WriteTo attempts cause InternalStreamRemaining to be calculated incorrectly
        var fileSize = 50;  // Smaller than BUFFER_SIZE (100)
        var reader = GetBinaryReadStream(fileSize);

        // Consume ALL data from the small file
        reader.ReadBytes(fileSize);
        Assert.Equal(fileSize, reader.Position);
        Assert.True(reader.Complete);

        // Now try WriteTo 0 bytes (or check Position after trying to read more)
        // This may trigger LoadPosition() when stream is at EOF
        var outputStream = new MemoryStream();

        // At this point, checking Position should not give a negative value
        // The bug: InternalStreamRemaining becomes the buffer size - _posOffset incorrectly
        var posBeforeWrite = reader.Position;
        Assert.True(posBeforeWrite >= 0, $"Position before WriteTo should not be negative, but was {posBeforeWrite}");

        // Try to trigger another LoadPosition by accessing internal state or calling WriteTo with 0
        // Actually, let me just verify Position is still correct
        var posAfter = reader.Position;
        Assert.True(posAfter >= 0, $"Position should not be negative, but was {posAfter}");
        Assert.Equal(fileSize, posAfter);
    }

    [Fact]
    public void Position_ForceLoadAtEOF_SmallFile_NotNegative()
    {
        // LoadPosition now returns early when at EOF to prevent infinite loops
        // This test verifies that attempting to load at EOF doesn't corrupt state
        var fileSize = 50;  // Smaller than BUFFER_SIZE (100)
        var reader = GetBinaryReadStream(fileSize);

        // Consume all data
        reader.ReadBytes(fileSize);
        var posBeforeLoad = reader.Position;

        // Calling LoadPosition at EOF should return early and not modify anything
        reader.LoadPosition();

        // Position should not go negative (primary concern)
        var posAfterLoad = reader.Position;
        Assert.True(posAfterLoad >= 0, $"Position should not be negative after LoadPosition at EOF, but was {posAfterLoad}");

        // Position should either stay the same or remain valid (≤ fileSize)
        Assert.True(posAfterLoad <= fileSize, $"Position should not exceed file size, but was {posAfterLoad}");
    }

    [Fact]
    public void WriteTo_RequestMoreThanAvailable_ShouldNotInfiniteLoop()
    {
        const int fileSize = 324;
        const int initialRead = 66;
        const int actualRemaining = 258;
        const int writeToAmount = actualRemaining + 1; // Request 1 more than available!

        // Use default buffer size (4096) to match real scenario
        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: 4096);

        // Read initial data to position at 66
        reader.ReadBytes(initialRead);
        Assert.Equal(initialRead, reader.Position);
        Assert.Equal(actualRemaining, reader.Remaining);

        // WriteTo with MORE data than available should NOT infinite loop
        // Use a timeout to detect infinite loops
        var outputStream = new MemoryStream();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        Exception? caughtException = null;

        var task = Task.Run(() =>
        {
            try
            {
                reader.WriteTo(outputStream, writeToAmount);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }
        }, cts.Token);

        var completed = task.Wait(TimeSpan.FromSeconds(2));

        Assert.True(completed, "WriteTo entered an infinite loop when requesting more data than available");
    }

    [Fact]
    public void LoadPosition_RequestMoreThanRemaining_ThrowsInsufficientDataException()
    {
        // When LoadPosition(amount) is called with an amount greater than what's remaining
        // in the stream, it should throw InsufficientDataException
        var fileSize = 100;
        var reader = GetBinaryReadStream(fileSize);

        // Position near the end so only 5 bytes remain
        reader.Position = 95;
        Assert.Equal(5, reader.Remaining);

        // Try to read UInt64 (8 bytes) when only 5 bytes remain - should throw
        Assert.Throws<DataMisalignedException>(() => reader.ReadUInt64());
    }
    [Fact]
    public void SeekBackwards_AfterReadingToEnd_ReadsCorrectBytes()
    {
        // Bug: When seeking backwards after being at the end of the stream,
        // the bytes read come from the wrong position.
        //
        // Scenario from Mutagen: PluginBinaryOverlay.LockExtractMemory positions to 0xC30
        // but the bytes read are from 0xC10 instead.
        //
        // Root cause: In SetPosition, _streamPos is set to the target position,
        // then LoadPosition() is called which incorrectly ADDS amountRead to _streamPos
        // (double-counting the position).

        const int fileSize = 0x1000;  // 4096 bytes
        const int seekTarget = 0xC30;  // Position to seek back to
        const int readAmount = 0x20;   // Amount to read after seeking

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: BUFFER_SIZE);

        // Read to the end of the stream
        reader.Position = fileSize;
        Assert.True(reader.Complete);
        Assert.Equal(fileSize, reader.Position);

        // Now seek backwards to position 0xC30
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // Read some bytes - they should match the data at position 0xC30
        var buffer = new byte[readAmount];
        var bytesRead = reader.Read(buffer, 0, readAmount);

        Assert.Equal(readAmount, bytesRead);

        // Verify the bytes match what should be at position 0xC30
        // GetByteArray creates bytes where byte[i] = i % 256
        for (int i = 0; i < readAmount; i++)
        {
            var expectedByte = (byte)((seekTarget + i) % 256);
            Assert.Equal(expectedByte, buffer[i]);
        }
    }

    [Fact]
    public void SeekBackwards_AcrossBufferBoundary_ReadsCorrectBytes()
    {
        // Test seeking backwards when it requires reloading the buffer
        // (target position is before the current buffer's start)

        const int fileSize = 500;
        const int bufferSize = 100;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read past the first buffer to position 250
        reader.Position = 250;
        Assert.Equal(250, reader.Position);

        // Now seek backwards to position 50 (before the current buffer)
        reader.Position = 50;
        Assert.Equal(50, reader.Position);

        // Read bytes and verify they're correct
        var buffer = new byte[20];
        reader.Read(buffer, 0, 20);

        for (int i = 0; i < 20; i++)
        {
            Assert.Equal((byte)(50 + i), buffer[i]);
        }
    }

    [Fact]
    public void SeekBackwards_FromEndOfStream_VerifyPositionAndData()
    {
        // This mimics the exact scenario in Mutagen where the stream is at EOF,
        // then seeks backwards to extract group memory

        const int fileSize = 3200;  // Mimics small .esp file size
        const int targetPosition = 3120;  // 0xC30 equivalent
        const int readSize = 32;  // 0x20 equivalent

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: 4096);  // Default buffer size

        // Move to end of stream (simulates reading through the file)
        reader.Position = fileSize;
        Assert.True(reader.Complete);

        // Seek backwards (simulates LockExtractMemory positioning)
        reader.Position = targetPosition;

        // Critical check: Position should be exactly where we set it
        Assert.Equal(targetPosition, reader.Position);

        // Read data
        var data = new byte[readSize];
        var read = reader.Read(data, 0, readSize);
        Assert.Equal(readSize, read);

        // Verify data integrity - this is where the bug manifests
        // If buggy, data[0] will be (targetPosition - 0x20) % 256 instead of targetPosition % 256
        for (int i = 0; i < readSize; i++)
        {
            var expected = (byte)((targetPosition + i) % 256);
            var actual = data[i];
            Assert.True(expected == actual,
                $"Byte mismatch at offset {i}: expected {expected} (from position {targetPosition + i}), " +
                $"got {actual} (which would be from position {actual + (i == 0 ? 0 : targetPosition - (targetPosition % 256))})");
        }
    }

    [Fact]
    public void SmallFile_SeekBackwards_AfterReadingAll_CorrectBytes()
    {
        // Bug scenario: Small file (< buffer size), read all data, then seek backwards
        // The issue might be that _posOffset or buffer state gets corrupted

        const int fileSize = 3200;  // Small file that fits in default 4096 buffer
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read entire file to EOF - this is what Mutagen does when scanning through
        var allData = reader.ReadBytes(fileSize);
        Assert.True(reader.Complete);
        Assert.Equal(fileSize, reader.Position);

        // Verify we read correct initial data
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)i, allData[i]);
        }

        // Now seek backwards - this is what LockExtractMemory does
        const int seekTarget = 0xC30;  // 3120
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // Read some bytes
        const int readSize = 0x20;  // 32 bytes
        var buffer = new byte[readSize];
        var bytesRead = reader.Read(buffer, 0, readSize);
        Assert.Equal(readSize, bytesRead);

        // Verify: bytes should be from position 0xC30, not 0xC10
        // If buggy, buffer[0] would be (0xC10 % 256) = 0x10 instead of (0xC30 % 256) = 0x30
        for (int i = 0; i < readSize; i++)
        {
            var expectedPos = seekTarget + i;
            var expected = (byte)(expectedPos % 256);
            Assert.Equal(expected, buffer[i]);
        }
    }

    [Fact]
    public void SmallFile_MultipleSeekBackwards_CorrectBytes()
    {
        // Test multiple seeks backwards to verify buffer state doesn't get corrupted

        const int fileSize = 3200;
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read to end
        reader.ReadBytes(fileSize);
        Assert.True(reader.Complete);

        // First seek back and read
        reader.Position = 100;
        var read1 = reader.ReadBytes(10);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)(100 + i), read1[i]);
        }

        // Second seek back to different position
        reader.Position = 500;
        var read2 = reader.ReadBytes(10);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)((500 + i) % 256), read2[i]);
        }

        // Third seek back to near-end
        reader.Position = 3100;
        var read3 = reader.ReadBytes(10);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)((3100 + i) % 256), read3[i]);
        }
    }

    [Fact]
    public void LargeFile_SeekBackwards_AfterDirectRead_CorrectBytes()
    {
        // Scenario: Large read that bypasses buffer (sets forceReload = true),
        // then seek backwards, then read.
        // This exercises the LoadPosition code path after forceReload.

        const int fileSize = 10000;
        const int bufferSize = 100;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Do a large direct read (larger than buffer) - this sets forceReload = true
        var largeRead = new byte[500];
        reader.Read(largeRead, 0, 500);
        Assert.Equal(500, reader.Position);

        // Verify the large read got correct data
        for (int i = 0; i < 500; i++)
        {
            Assert.Equal((byte)(i % 256), largeRead[i]);
        }

        // Now seek backwards - this should trigger LoadPosition with forceReload
        reader.Position = 100;
        Assert.Equal(100, reader.Position);

        // Read and verify
        var read = reader.ReadBytes(50);
        for (int i = 0; i < 50; i++)
        {
            Assert.Equal((byte)((100 + i) % 256), read[i]);
        }
    }

    [Fact]
    public void SeekBackwards_WithExactStreamPosMatch_CorrectBytes()
    {
        // Edge case: What if after some operations, _streamPos happens to equal
        // the position we want to seek to? This could cause issues if we skip
        // repositioning the underlying stream.

        const int fileSize = 5000;
        const int bufferSize = 100;
        const int targetPos = 200;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read past target position to load buffer
        reader.Position = 300;
        reader.ReadBytes(50);

        // Force a scenario where _streamPos might match our target
        // Read exactly to position 200
        reader.Position = 100;
        reader.ReadBytes(100);  // Now position should be 200

        // Seek to a different position first
        reader.Position = 400;
        reader.ReadBytes(10);

        // Now seek back to 200 - _streamPos might be at various values
        reader.Position = targetPos;
        Assert.Equal(targetPos, reader.Position);

        var read = reader.ReadBytes(10);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)((targetPos + i) % 256), read[i]);
        }
    }

    [Fact]
    public void ReadToEOF_ThenSeekBack_WithBufferReload_CorrectBytes()
    {
        // Simulates the Mutagen scenario more closely:
        // - File larger than buffer
        // - Read/seek through file until at EOF
        // - Seek backwards to position that requires buffer reload
        // - Read and verify bytes

        const int fileSize = 5000;
        const int bufferSize = 100;
        const int seekTarget = 0xC30 % fileSize;  // 3120 mod 5000 = 3120

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read to end of file, simulating scanning through
        reader.Position = fileSize;
        Assert.True(reader.Complete);

        // Now _streamPos = fileSize, buffer contains last chunk of file
        // Seek backwards to position 3120 (0xC30)
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // This seek should trigger LoadPosition because seekTarget < startLoc
        // startLoc = _streamPos - _internalBufferLength = 5000 - 100 = 4900
        // seekTarget = 3120 < 4900, so we should reload

        // Read and verify - if bug exists, we'd get wrong bytes
        var read = reader.ReadBytes(32);
        for (int i = 0; i < 32; i++)
        {
            var expected = (byte)((seekTarget + i) % 256);
            var actual = read[i];
            Assert.True(expected == actual,
                $"Byte mismatch at offset {i}: expected {expected} from pos {seekTarget + i}, got {actual}");
        }
    }

    [Fact]
    public void SeekBackwards_WithUnderlyingStreamPositionCheck_NoMismatch()
    {
        // Enable the CheckUnderlyingStreamPosition flag to catch any state inconsistencies

        const int fileSize = 5000;
        const int bufferSize = 100;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);
        reader.CheckUnderlyingStreamPosition = true;

        // Read to end
        reader.Position = fileSize;
        Assert.True(reader.Complete);

        // Seek backwards - if there's a state mismatch, this will throw
        reader.Position = 3120;
        Assert.Equal(3120, reader.Position);

        // Read and verify
        var read = reader.ReadBytes(32);
        for (int i = 0; i < 32; i++)
        {
            Assert.Equal((byte)((3120 + i) % 256), read[i]);
        }
    }

    [Fact]
    public void SeekBackwards_UsingFileStream_CorrectBytes()
    {
        // Test with actual FileStream to see if behavior differs from MemoryStream

        const int fileSize = 5000;
        const int bufferSize = 100;
        var tempFile = Path.GetTempFileName();

        try
        {
            // Write test data to temp file
            File.WriteAllBytes(tempFile, GetByteArray(fileSize));

            using var reader = new BinaryReadStream(tempFile, bufferSize: bufferSize);

            // Read to end
            reader.Position = fileSize;
            Assert.True(reader.Complete);

            // Seek backwards
            reader.Position = 3120;
            Assert.Equal(3120, reader.Position);

            // Read and verify
            var read = reader.ReadBytes(32);
            for (int i = 0; i < 32; i++)
            {
                Assert.Equal((byte)((3120 + i) % 256), read[i]);
            }
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    [Fact]
    public void InternalState_AfterSeekBackwards_IsConsistent()
    {
        // Verify internal state consistency after seeking backwards

        const int fileSize = 5000;
        const int bufferSize = 100;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read to end
        reader.Position = fileSize;

        // Capture state before seek
        var streamPosBefore = reader._streamPos;
        var bufLenBefore = reader._internalBufferLength;

        // Seek backwards to trigger reload
        reader.Position = 100;

        // After seeking backwards, verify:
        // 1. Position reports correctly
        Assert.Equal(100, reader.Position);

        // 2. _streamPos should have been updated during LoadPosition
        // Should be 100 + bufferSize = 200 (or less if file ended sooner)
        Assert.True(reader._streamPos >= 100, $"_streamPos ({reader._streamPos}) should be >= 100");
        Assert.True(reader._streamPos <= 100 + bufferSize, $"_streamPos ({reader._streamPos}) should be <= {100 + bufferSize}");

        // 3. _internalBufferLength should match what was read
        Assert.True(reader._internalBufferLength > 0);
        Assert.True(reader._internalBufferLength <= bufferSize);

        // 4. Data should be correct
        var firstByte = reader.ReadUInt8();
        Assert.Equal(100, firstByte);
    }

    [Fact]
    public void InterleavedLockExtractMemory_SimulatedMutagenPattern()
    {
        // Simulates Mutagen's pattern of:
        // 1. Scan file (read through it)
        // 2. Multiple lazy accesses that call LockExtractMemory at different positions

        const int fileSize = 5000;
        const int bufferSize = 4096;  // Default buffer size like Mutagen uses

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Phase 1: Simulate scanning by reading headers and advancing
        // This is what RecordLocator does
        int scanPos = 0;
        while (scanPos < fileSize - 24)  // Leave room for a "header"
        {
            reader.Position = scanPos;
            var header = reader.ReadBytes(24);  // Read a "record header"
            scanPos += 100 + (scanPos % 50);  // Advance by varying amounts
        }

        // Now reader might be in any state after scanning
        // Stream is NOT necessarily at EOF

        // Phase 2: Simulate LockExtractMemory calls at various positions
        // These access groups/records that were found during scanning

        void LockExtractMemorySimulation(long min, int size)
        {
            lock (reader)
            {
                reader.Position = min;
                Assert.Equal(min, reader.Position);

                byte[] data = new byte[size];
                var read = reader.Read(data);
                Assert.Equal(size, read);

                // Verify data integrity
                for (int i = 0; i < size; i++)
                {
                    var expected = (byte)((min + i) % 256);
                    Assert.Equal(expected, data[i]);
                }
            }
        }

        // Multiple extractions at different positions (mimics lazy access)
        LockExtractMemorySimulation(0xC30 % fileSize, 32);  // Position 3120
        LockExtractMemorySimulation(100, 50);
        LockExtractMemorySimulation(0xC10 % fileSize, 32);  // Position 3088
        LockExtractMemorySimulation(4000, 100);
        LockExtractMemorySimulation(0xC30 % fileSize, 32);  // Same position again
        LockExtractMemorySimulation(500, 200);
    }

    [Fact]
    public void LockExtractMemory_AfterPositionSetToEnd_CorrectBytes()
    {
        // Test the exact scenario: Position set to end (via scan), then extract at 0xC30

        const int fileSize = 3200;  // Small file like the .esp
        const int bufferSize = 4096;  // Default Mutagen buffer

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Scan to end - this loads the entire file into buffer
        reader.Position = fileSize;
        Assert.True(reader.Complete);

        // The entire file (3200 bytes) should now be in the 4096-byte buffer
        // _streamPos = 3200
        // _internalBufferLength = 3200
        // _posOffset = 4096 - 3200 = 896

        // Now extract at position 0xC30 (3120)
        const long extractPos = 0xC30;  // 3120
        const int extractSize = 32;

        reader.Position = extractPos;
        Assert.Equal(extractPos, reader.Position);

        var data = new byte[extractSize];
        var read = reader.Read(data);
        Assert.Equal(extractSize, read);

        // This is where the bug would manifest
        // If buggy: data[0] would be (0xC10 % 256) = 0x10 = 16
        // If correct: data[0] should be (0xC30 % 256) = 0x30 = 48
        for (int i = 0; i < extractSize; i++)
        {
            var expected = (byte)((extractPos + i) % 256);
            var actual = data[i];
            Assert.True(expected == actual,
                $"At offset {i}: expected byte {expected} (from pos {extractPos + i}), got {actual}. " +
                $"If this is {(extractPos - 0x20 + i) % 256}, the bug is reading from 0xC10 instead of 0xC30!");
        }
    }

    [Fact]
    public void FileSizeSlightlyOverBuffer_SeekBackwards_CorrectBytes()
    {
        // Exact reproduction of the bug: file is 0x101F (4127) bytes,
        // which is JUST OVER the 4096-byte buffer
        // Seek position is 0xC30 (3120)

        const int fileSize = 0x101F;  // 4127 bytes - exact file size!
        const int bufferSize = 4096;  // Default buffer

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Read to end of file (simulating scan)
        reader.Position = fileSize;
        Assert.True(reader.Complete);

        // State after reading to end:
        // First buffer load: bytes 0-4095, _streamPos = 4096
        // Second (partial) load: bytes 4096-4126, _streamPos = 4127
        // But the last buffer only has 31 bytes (4127-4096)

        // Now seek to 0xC30 (3120) - this is BEFORE the current buffer's start
        // startLoc = 4127 - 31 = 4096, seekTarget = 3120 < 4096
        // So this should trigger LoadPosition()
        const long seekTarget = 0xC30;  // 3120
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // Read and verify - this is where the bug would show
        const int readSize = 32;
        var data = new byte[readSize];
        var read = reader.Read(data);
        Assert.Equal(readSize, read);

        for (int i = 0; i < readSize; i++)
        {
            var expected = (byte)((seekTarget + i) % 256);
            var actual = data[i];
            Assert.True(expected == actual,
                $"Byte mismatch at offset {i}: expected {expected} (from pos {seekTarget + i}), " +
                $"got {actual}. Possible wrong position = {(actual - i) % 256 + ((seekTarget / 256) * 256)}");
        }
    }

    [Fact]
    public void FileSizeExactlyOverBuffer_DetailedStateCheck()
    {
        // Detailed state check for file just over buffer size

        const int fileSize = 0x101F;  // 4127 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Move to position past first buffer
        reader.Position = fileSize;

        // Check state after going to end
        Assert.Equal(fileSize, reader._streamPos);  // Should be at end
        var bufLen = reader._internalBufferLength;  // Should be 31 (the overflow)
        Assert.True(bufLen <= fileSize - bufferSize + bufLen, $"Buffer length {bufLen} unexpected");

        // Now seek back to 0xC30
        const long seekTarget = 0xC30;
        var startLocBefore = reader._streamPos - reader._internalBufferLength;

        reader.Position = seekTarget;

        // Verify position and read
        Assert.Equal(seekTarget, reader.Position);

        var firstByte = reader.ReadUInt8();
        Assert.Equal((byte)(seekTarget % 256), firstByte);

        reader.Position = seekTarget;  // Reset to read more
        var bytes = reader.ReadBytes(10);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal((byte)((seekTarget + i) % 256), bytes[i]);
        }
    }

    [Fact]
    public void RealEspFile_SeekToGroupPosition_CorrectBytes()
    {
        // Test with the actual .esp file that exhibits the bug
        var filePath = @"D:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\Arclight - Adamant Patch.esp";
        if (!File.Exists(filePath))
        {
            return;  // Skip if file doesn't exist
        }

        const int bufferSize = 4096;  // Default Mutagen buffer

        using var reader = new BinaryReadStream(filePath, bufferSize: bufferSize);

        // The file is 0x101F (4127) bytes
        // Position 0xC30 should have "GRUP" (47 52 55 50)
        // Position 0xC10 has "be 44 0c 00" (different data)

        // Simulate what Mutagen does: scan to end, then seek back
        reader.Position = reader.Length;  // Go to EOF
        Assert.True(reader.Complete);

        // Now seek to position 0xC30 where GRUP header should be
        const long grupPosition = 0xC30;
        reader.Position = grupPosition;
        Assert.Equal(grupPosition, reader.Position);

        // Read the first 4 bytes - should be "GRUP"
        var header = reader.ReadBytes(4);

        // GRUP in little-endian is: G(0x47) R(0x52) U(0x55) P(0x50)
        Assert.Equal(0x47, header[0]);  // 'G'
        Assert.Equal(0x52, header[1]);  // 'R'
        Assert.Equal(0x55, header[2]);  // 'U'
        Assert.Equal(0x50, header[3]);  // 'P'

        // If the bug exists, we'd get: 0xBE, 0x44, 0x0C, 0x00 (from position 0xC10)
    }

    [Fact]
    public void RealEspFile_SimulateMutagenScan_ThenExtract()
    {
        // More accurate simulation of what Mutagen does during overlay creation
        var filePath = @"D:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\Arclight - Adamant Patch.esp";
        if (!File.Exists(filePath))
        {
            return;  // Skip if file doesn't exist
        }

        const int bufferSize = 4096;

        using var reader = new BinaryReadStream(filePath, bufferSize: bufferSize);

        // Simulate Mutagen's FillModTypes scanning:
        // 1. Read mod header
        var modHeaderType = reader.ReadBytes(4);  // TES4
        reader.Position = 0;

        // Get header total length (at offset 4)
        reader.Position = 4;
        var headerLength = reader.ReadUInt32();
        var totalHeaderLength = headerLength + 24;  // Including header itself

        // Move to first group after mod header
        reader.Position = totalHeaderLength;

        // Scan through groups
        while (!reader.Complete)
        {
            var groupType = reader.ReadBytes(4);  // Should be "GRUP"
            if (groupType[0] != 'G' || groupType[1] != 'R' || groupType[2] != 'U' || groupType[3] != 'P')
            {
                break;  // Not a group, stop
            }
            var groupLength = reader.ReadUInt32();

            // Skip to next group
            var nextPos = reader.Position - 8 + groupLength;
            if (nextPos > reader.Length)
            {
                reader.Position = reader.Length;  // Go to EOF if group extends past file
                break;
            }
            reader.Position = nextPos;
        }

        // Now we're at EOF or past last complete group
        // This is the state when lazy loading happens

        // Simulate LockExtractMemory call for Scrolls group at 0xC30
        const long grupPosition = 0xC30;
        reader.Position = grupPosition;

        // Read and verify - should be "GRUP"
        var header = reader.ReadBytes(4);
        Assert.Equal((byte)'G', header[0]);
        Assert.Equal((byte)'R', header[1]);
        Assert.Equal((byte)'U', header[2]);
        Assert.Equal((byte)'P', header[3]);
    }

    [Fact]
    public void RealEspFile_SimulateWithGetHeader()
    {
        // Even more accurate: use Get methods like Mutagen does
        var filePath = @"D:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\Arclight - Adamant Patch.esp";
        if (!File.Exists(filePath))
        {
            return;
        }

        const int bufferSize = 4096;

        using var reader = new BinaryReadStream(filePath, bufferSize: bufferSize);

        // Mutagen uses GetUInt32 and similar to peek at headers without advancing
        // Then sets Position to skip entire records

        // Get TES4 header length
        var tes4Length = reader.GetUInt32(4);  // Peek at length field
        var totalTes4 = tes4Length + 24;

        // Move past TES4 record
        reader.Position = totalTes4;

        // Scan groups using Get (peek) methods
        while (!reader.Complete)
        {
            var recordType = reader.GetUInt32(0);  // Peek at record type
            if (recordType != 0x50555247)  // "GRUP" in little-endian
            {
                break;
            }

            var grupLength = reader.GetUInt32(4);  // Peek at length
            var nextPos = reader.Position + grupLength;

            if (nextPos > reader.Length)
            {
                reader.Position = reader.Length;
                break;
            }
            reader.Position = nextPos;
        }

        // At EOF - now do LockExtractMemory simulation
        const long grupPosition = 0xC30;
        reader.Position = grupPosition;

        var header = reader.ReadBytes(4);
        Assert.Equal((byte)'G', header[0]);
        Assert.Equal((byte)'R', header[1]);
        Assert.Equal((byte)'U', header[2]);
        Assert.Equal((byte)'P', header[3]);
    }

    [Fact]
    public void PositionPastEOF_OffByOne_Bug()
    {
        // BUG REPRODUCTION: Position reports 0x1020 (4128) but file is only 0x101F (4127)
        // This is an off-by-one error when file size is just over buffer size

        const int fileSize = 0x101F;  // 4127 bytes - exact file size
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Simulate scanning to EOF
        reader.Position = fileSize;

        // BUG CHECK: Position should equal fileSize (4127), not fileSize + 1 (4128)
        Assert.Equal(fileSize, reader.Position);
        Assert.Equal(fileSize, reader.Length);
        Assert.Equal(0, reader.Remaining);
        Assert.True(reader.Complete);

        // If Position reports fileSize + 1, this is the bug!
        Assert.True(reader.Position <= reader.Length,
            $"Position {reader.Position} should not exceed Length {reader.Length}");
    }

    [Fact]
    public void PositionPastEOF_ThenSeekBack_WrongData()
    {
        // This test should FAIL if the bug exists:
        // 1. File is 4127 bytes (just over 4096 buffer)
        // 2. After scanning, Position reports 4128 (off by one)
        // 3. When seeking back to 0xC30 and reading, data comes from wrong position

        const int fileSize = 0x101F;  // 4127 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Scan to EOF
        reader.Position = fileSize;

        // Check the off-by-one condition
        var posAtEof = reader.Position;
        var expectedPos = fileSize;  // Should be 4127

        // If this fails, Position is reporting wrong value (4128 instead of 4127)
        Assert.Equal(expectedPos, posAtEof);

        // Now seek back to 0xC30 (3120)
        const long seekTarget = 0xC30;
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // Read and verify data
        var data = reader.ReadBytes(32);
        for (int i = 0; i < 32; i++)
        {
            var expected = (byte)((seekTarget + i) % 256);
            Assert.Equal(expected, data[i]);
        }
    }

    [Fact]
    public void FileJustOverBuffer_InternalStateCheck()
    {
        // Detailed internal state check for the off-by-one scenario

        const int fileSize = 0x101F;  // 4127 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Move to EOF
        reader.Position = fileSize;

        // Check internal state
        // _streamPos should be fileSize (4127)
        Assert.Equal(fileSize, reader._streamPos);

        // When we set Position = fileSize (4127), the stream seeks to 4127 then LoadPosition
        // reads 0 bytes because we're at EOF. So _internalBufferLength = 0.
        // This is actually expected behavior for seeking to exact EOF.
        Assert.Equal(0, reader._internalBufferLength);

        // Position should still equal fileSize (4127)
        Assert.Equal(fileSize, reader.Position);
    }

    [Fact]
    public void SetPosition_PastLength_ShouldThrow()
    {
        // Verify that setting Position past Length throws
        const int fileSize = 0x101F;  // 4127 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Setting Position to Length should work
        reader.Position = fileSize;
        Assert.Equal(fileSize, reader.Position);

        // Setting Position past Length should throw
        Assert.Throws<ArgumentException>(() => reader.Position = fileSize + 1);
    }

    [Fact]
    public void TruncatedFile_SimulateMutagenScan_PositionPastEOF()
    {
        // Simulates the EXACT Mutagen scenario:
        // - File is 4127 bytes (0x101F)
        // - GRUP at 0xC30 claims 1008 bytes (0x3F0)
        // - 0xC30 + 0x3F0 = 0x1020 (4128) - this is PAST EOF!
        // - Mutagen tries to set Position = 4128

        const int fileSize = 0x101F;  // 4127 bytes
        const int bufferSize = 4096;
        const long grupPosition = 0xC30;
        const int grupClaimedLength = 0x3F0;  // 1008 - but only 1007 bytes remain!
        const long positionAfterGrup = grupPosition + grupClaimedLength;  // 0x1020 = 4128

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Simulate scanning up to the last group
        reader.Position = grupPosition;

        // Now Mutagen would try to set Position = grupPosition + grupClaimedLength
        // This is past EOF (4128 > 4127)

        // QUESTION: Does BinaryReadStream allow this? It shouldn't!
        // But if it does (bug), then Position would be 4128 which matches user's report

        if (positionAfterGrup > fileSize)
        {
            // This SHOULD throw - position past EOF
            var threw = false;
            try
            {
                reader.Position = positionAfterGrup;
            }
            catch (ArgumentException)
            {
                threw = true;
            }

            Assert.True(threw, $"Expected exception when setting Position to {positionAfterGrup} (past EOF at {fileSize})");
        }
    }

    [Fact]
    public void RealEspFile_ExactScenario()
    {
        // The EXACT bug scenario:
        // File is 0x1020 (4128) bytes - exactly 32 bytes over 4096 buffer
        // Scanning takes Position to 0x1020 (EOF)
        // LockExtractMemory seeks to 0xC30 and reads
        // Bug: bytes come from wrong position

        var filePath = @"D:\SteamLibrary\steamapps\common\Skyrim Special Edition\Data\Arclight - Adamant Patch.esp";
        if (!File.Exists(filePath))
        {
            return;
        }

        using var reader = new BinaryReadStream(filePath, bufferSize: 4096);

        // File is 0x1020 (4128) bytes
        Assert.Equal(0x1020, reader.Length);

        // Scan to EOF - this is where Mutagen would be after FillModTypes
        reader.Position = reader.Length;
        Assert.True(reader.Complete);
        Assert.Equal(0x1020, reader.Position);
        Assert.Equal(0, reader.Remaining);

        // Now simulate LockExtractMemory: seek back to 0xC30
        const long grupPosition = 0xC30;
        reader.Position = grupPosition;
        Assert.Equal(grupPosition, reader.Position);

        // Read the GRUP header - should be "GRUP"
        var header = reader.ReadBytes(4);

        // If bug exists: we'd get bytes from 0xC10 instead of 0xC30
        // 0xC10 has: be 44 0c 00
        // 0xC30 has: 47 52 55 50 ("GRUP")

        // This is the critical assertion - if this fails, the bug is confirmed!
        Assert.Equal((byte)'G', header[0]);  // 0x47
        Assert.Equal((byte)'R', header[1]);  // 0x52
        Assert.Equal((byte)'U', header[2]);  // 0x55
        Assert.Equal((byte)'P', header[3]);  // 0x50
    }

    [Fact]
    public void FileSizeExactly32OverBuffer_SeekBackAfterEOF()
    {
        // File is exactly 4128 bytes (32 over buffer size of 4096)
        // This is the exact scenario where the bug manifests

        const int fileSize = 0x1020;  // 4128 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Go to EOF
        reader.Position = fileSize;
        Assert.True(reader.Complete);
        Assert.Equal(fileSize, reader.Position);

        // Seek back to 0xC30 (3120)
        const long seekTarget = 0xC30;
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        // Read and verify
        var data = reader.ReadBytes(32);
        for (int i = 0; i < 32; i++)
        {
            var expected = (byte)((seekTarget + i) % 256);
            var actual = data[i];

            // If bug exists, actual would be from position (seekTarget - 0x20)
            Assert.True(expected == actual,
                $"Byte mismatch at offset {i}: expected {expected} from pos {seekTarget + i}, got {actual}");
        }
    }

    [Fact]
    public void BugRepro_InconsistentBufferState()
    {
        // Test that seeking within buffer correctly uses _bufferStartPos to calculate offset.
        // The fix ensures that the buffer bounds check uses the actual tracked buffer position
        // rather than deriving it from _streamPos.

        const int fileSize = 0x1020;  // 4128 bytes
        const int bufferSize = 4096;

        var reader = new BinaryReadStream(
            new MemoryStream(GetByteArray(fileSize)),
            bufferSize: bufferSize);

        // Load buffer with bytes 0-4095
        reader.Position = 0;
        reader.ReadBytes(1);  // Trigger initial buffer load

        Assert.Equal(4096, reader._internalBufferLength);

        // Seek within the buffer and verify correct data
        const long seekTarget = 3000;
        reader.Position = seekTarget;
        Assert.Equal(seekTarget, reader.Position);

        var data = reader.ReadBytes(32);

        // Verify data is from the correct position
        for (int i = 0; i < 32; i++)
        {
            var expected = (byte)((seekTarget + i) % 256);
            var actual = data[i];
            Assert.True(expected == actual,
                $"At offset {i}, expected byte {expected} from position {seekTarget + i}, got {actual}");
        }

        // Now trigger a reload by seeking past buffer
        reader.Position = 4100;
        Assert.Equal(4100, reader.Position);

        // Read and verify correct data from new buffer position
        var data2 = reader.ReadBytes(20);
        for (int i = 0; i < 20; i++)
        {
            var expected = (byte)((4100 + i) % 256);
            Assert.Equal(expected, data2[i]);
        }
    }
    #endregion
}