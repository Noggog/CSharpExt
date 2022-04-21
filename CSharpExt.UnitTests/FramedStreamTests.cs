using FluentAssertions;
using Noggog.Streams;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Xunit;

namespace CSharpExt.UnitTests;

public class FramedStreamTests
{
    [Theory, TestData]
    public void FramedStreamLonger(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        var frame = new FramedStream(framedStream, 6);
        frame.Length.Should().Be(6);
        frame.Position.Should().Be(0);
    }

    [Theory, TestData]
    public void FramedStreamShorter(Stream framedStream)
    {
        framedStream.Length.Returns(4);
        var frame = new FramedStream(framedStream, 6);
        frame.Length.Should().Be(4);
        frame.Position.Should().Be(0);
    }

    [Theory, TestData]
    public void FramedStreamShorterDueToPos(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 8;
        var frame = new FramedStream(framedStream, 6);
        frame.Length.Should().Be(2);
        frame.Position.Should().Be(0);
    }

    [Theory, TestData]
    public void FramedStreamUnknown(Stream framedStream)
    {
        framedStream.Length.Returns(-1);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6);
        frame.Length.Should().Be(6);
        frame.Position.Should().Be(0);
    }

    [Theory, TestData]
    public void FramedPositionIncrement(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6);
        frame.Length.Should().Be(6);
        frame.Position.Should().Be(0);
        framedStream.Position = 6;
        frame.Length.Should().Be(6);
        frame.Position.Should().Be(6);
    }

    [Theory, TestData]
    public void FramedPositionSet(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6);
        frame.Position += 2;
        framedStream.Position.Should().Be(2);
    }

    [Theory, TestData]
    public void Flush(Stream framedStream)
    {
        var frame = new FramedStream(framedStream, 6);
        frame.Flush();
        framedStream.Received(1).Flush();
    }

    [Fact]
    public void ReadOutOfRange()
    {
        byte[] bufIn = new byte[10];
        for (byte i = 0; i < 10; i++)
        {
            bufIn[i] = i;
        }

        var framedStream = new MemoryStream(bufIn);
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        byte[] buf = new byte[100];
        frame.Read(buf, 0, 7)
            .Should().Be(6);
        for (byte i = 0; i < 6; i++)
        {
            buf[i].Should().Be(i);
        }

        for (int i = 6; i < 100; i++)
        {
            buf[i].Should().Be(0);
        }
    }

    [Theory, TestData]
    public void Read(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        const int offset = 5;
        const int count = 3;
        byte[] b = new byte[10];
        frame.Read(b, offset, count);
        framedStream.Received(1).Read(b, offset, 3);
    }

    [Theory, TestData]
    public void ReadAll(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        const int offset = 5;
        byte[] b = new byte[10];
        frame.Read(b, offset, (int)frame.Length);
        framedStream.Received(1).Read(b, offset, (int) frame.Length);
    }

    [Theory, TestData]
    public void WriteOutOfRange(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        Action act = () => frame.Write(Array.Empty<byte>(), 0, 7);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory, TestData]
    public void Write(Stream framedStream)
    {
        framedStream.Length.Returns(10);
        framedStream.Position = 0;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        const int offset = 5;
        const int count = 3;
        byte[] b = new byte[10];
        frame.Write(b, offset, count);
        framedStream.Received(1).Write(b, offset, 3);
    }

    [Theory, TestData]
    public void PositionSet(Stream framedStream)
    {
        framedStream.Position = 3;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        frame.Position += 10;
        frame.Position.Should().Be(10);
        framedStream.Position.Should().Be(13);
    }

    [Theory, TestData]
    public void SeekBegin(Stream framedStream)
    {
        framedStream.Position = 3;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        frame.Seek(10, SeekOrigin.Begin);
        framedStream.Position.Should().Be(13);
    }

    [Theory, TestData]
    public void SeekCurrent(Stream framedStream)
    {
        framedStream.Position = 3;
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        frame.Position = 3;
        frame.Seek(10, SeekOrigin.Current);
        frame.Position.Should().Be(13);
        framedStream.Position.Should().Be(16);
    }

    [Theory, TestData]
    public void SeekEnd(Stream framedStream)
    {
        framedStream.Position = 3;
        framedStream.Length.Returns(10);
        var frame = new FramedStream(framedStream, 6, doDispose: false);
        frame.Seek(-3, SeekOrigin.End);
        frame.Position.Should().Be(3);
        framedStream.Position.Should().Be(6);
    }
}