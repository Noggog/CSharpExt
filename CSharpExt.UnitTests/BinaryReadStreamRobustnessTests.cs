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
}
