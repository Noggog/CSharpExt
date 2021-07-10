using System;
using System.IO;
using FakeItEasy;
using FluentAssertions;
using Noggog.Streams;
using Noggog.Testing.AutoFixture;
using Xunit;

namespace CSharpExt.UnitTests
{
    public class FramedStreamTests
    {
        [Theory, AutoFakeItEasyData(false)]
        public void FramedStreamLonger(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void FramedStreamShorter(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(4);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(4);
            frame.Position.Should().Be(0);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void FramedStreamShorterDueToPos(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            A.CallTo(() => framedStream.Position).Returns(8);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(2);
            frame.Position.Should().Be(0);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void FramedStreamUnknown(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(-1);
            A.CallTo(() => framedStream.Position).Returns(0);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void FramedPositionIncrement(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            A.CallTo(() => framedStream.Position).Returns(0);
            var frame = new FramedStream(framedStream, 6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(0);
            A.CallTo(() => framedStream.Position).Returns(6);
            frame.Length.Should().Be(6);
            frame.Position.Should().Be(6);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void FramedPositionSet(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            framedStream.Position = 0;
            var frame = new FramedStream(framedStream, 6);
            frame.Position += 2;
            framedStream.Position.Should().Be(2);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void Flush(Stream framedStream)
        {
            var frame = new FramedStream(framedStream, 6);
            frame.Flush();
            A.CallTo(() => framedStream.Flush()).MustHaveHappenedOnceExactly();
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

        [Theory, AutoFakeItEasyData(false)]
        public void Read(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            A.CallTo(() => framedStream.Position).Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            const int count = 3;
            byte[] b = new byte[10];
            frame.Read(b, offset, count);
            A.CallTo(() => framedStream.Read(b, offset, 3)).MustHaveHappenedOnceExactly();
        }

        [Theory, AutoFakeItEasyData(false)]
        public void ReadAll(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            A.CallTo(() => framedStream.Position).Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            byte[] b = new byte[10];
            frame.Read(b, offset, (int)frame.Length);
            A.CallTo(() => framedStream.Read(b, offset, (int)frame.Length))
                .MustHaveHappenedOnceExactly();
        }

        [Theory, AutoFakeItEasyData(false)]
        public void WriteOutOfRange(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            A.CallTo(() => framedStream.Position).Returns(0);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            Action act = () => frame.Write(Array.Empty<byte>(), 0, 7);
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory, AutoFakeItEasyData(false)]
        public void Write(Stream framedStream)
        {
            A.CallTo(() => framedStream.Length).Returns(10);
            framedStream.Position = 0;
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            const int offset = 5;
            const int count = 3;
            byte[] b = new byte[10];
            frame.Write(b, offset, count);
            A.CallTo(() => framedStream.Write(b, offset, 3))
                .MustHaveHappenedOnceExactly();
        }

        [Theory, AutoFakeItEasyData(false)]
        public void PositionSet(Stream framedStream)
        {
            framedStream.Position = 3;
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Position += 10;
            frame.Position.Should().Be(10);
            framedStream.Position.Should().Be(13);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void SeekBegin(Stream framedStream)
        {
            framedStream.Position = 3;
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Seek(10, SeekOrigin.Begin);
            framedStream.Position.Should().Be(13);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void SeekCurrent(Stream framedStream)
        {
            framedStream.Position = 3;
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Position = 3;
            frame.Seek(10, SeekOrigin.Current);
            frame.Position.Should().Be(13);
            framedStream.Position.Should().Be(16);
        }

        [Theory, AutoFakeItEasyData(false)]
        public void SeekEnd(Stream framedStream)
        {
            framedStream.Position = 3;
            A.CallTo(() => framedStream.Length).Returns(10);
            var frame = new FramedStream(framedStream, 6, doDispose: false);
            frame.Seek(-3, SeekOrigin.End);
            frame.Position.Should().Be(3);
            framedStream.Position.Should().Be(6);
        }
    }
}