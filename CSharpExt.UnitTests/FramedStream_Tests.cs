using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Noggog.Streams;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class FramedStream_Tests
    {
        [Fact]
        public void FramedStreamLonger()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
        }

        [Fact]
        public void FramedStreamShorter()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(4);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(4);
            frame.Position.Should().Be(0);
        }

        [Fact]
        public void FramedStreamShorterDueToPos()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(8);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(2);
            frame.Position.Should().Be(0);
        }

        [Fact]
        public void FramedStreamUnknown()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(-1);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
        }

        [Fact]
        public void FramedPositionIncrement()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
            framedStream.Position.Returns(6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(6);
        }

        [Fact]
        public void FramedPositionSet()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6);
            frame.Position += 2;
            framedStream.Position.Should().Be(2);
        }

        [Fact]
        public void Flush()
        {
            var framedStream = Substitute.For<Stream>();
            var frame = new FramedStream(framedStream, 6);
            frame.Flush();
            framedStream.Received().Flush();
        }

        [Fact]
        public void Disposes()
        {
            var framedStream = Substitute.For<Stream>();
            var frame = new FramedStream(framedStream, 6, doDispose: true);
            frame.Dispose();
            framedStream.Received().Dispose();
        }

        [Fact]
        public void NotDisposes()
        {
            var framedStream = Substitute.For<Stream>();
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Dispose();
            framedStream.DidNotReceive().Dispose();
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

        [Fact]
        public void Read()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            const int count = 3;
            byte[] b = new byte[10];
            frame.Read(b, offset, count);
            framedStream.Received().Read(b, offset, 3);
        }

        [Fact]
        public void ReadAll()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            byte[] b = new byte[10];
            frame.Read(b, offset, (int)frame.Length);
            framedStream.Received().Read(b, offset, (int)frame.Length);
        }

        [Fact]
        public void WriteOutOfRange()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            Action act = () => frame.Write(Array.Empty<byte>(), 0, 7);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Write()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Length.Returns(10);
            framedStream.Position.Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            const int count = 3;
            byte[] b = new byte[10];
            frame.Write(b, offset, count);
            framedStream.Received().Write(b, offset, 3);
        }

        [Fact]
        public void PositionSet()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Position.Returns(3);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Position += 10;
            frame.Position.Should().Be(10);
            framedStream.Position.Should().Be(13);
        }

        [Fact]
        public void SeekBegin()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Position.Returns(3);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Seek(10, SeekOrigin.Begin);
            framedStream.Position.Should().Be(13);
        }

        [Fact]
        public void SeekCurrent()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Position.Returns(3);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Position = 3;
            frame.Seek(10, SeekOrigin.Current);
            frame.Position.Should().Be(13);
            framedStream.Position.Should().Be(16);
        }

        [Fact]
        public void SeekEnd()
        {
            var framedStream = Substitute.For<Stream>();
            framedStream.Position.Returns(3);
            framedStream.Length.Returns(10);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Seek(-3, SeekOrigin.End);
            frame.Position.Should().Be(3);
            framedStream.Position.Should().Be(6);
        }
    }
}