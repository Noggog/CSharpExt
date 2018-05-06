using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public abstract class IBinaryStream_Tests
    {
        public const int TYPICAL_TEST_LENGTH = 56;
        public abstract IBinaryStream GetStream(int length);
        public abstract int EdgeLocation { get; }

        public byte[] GetByteArray(int size)
        {
            var ret = new byte[size];
            for (int i = 0; i < size; i++)
            {
                ret[i] = (byte)(i % 256);
            }
            return ret;
        }

        [Fact]
        public void Length()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
        }

        [Fact]
        public void ZeroPosition()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(0, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Remaining);
            Assert.False(stream.Complete);
        }

        [Fact]
        public void SetPosition()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 33;
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(33, stream.Position);
            Assert.Equal(23, stream.Remaining);
            Assert.False(stream.Complete);
        }

        [Fact]
        public void SetPosition_Negative()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            Assert.Throws<ArgumentException>(() => stream.Position = -2);
        }

        [Fact]
        public void Complete()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = TYPICAL_TEST_LENGTH;
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Position);
            Assert.Equal(0, stream.Remaining);
            Assert.True(stream.Complete);
        }

        [Fact]
        public void ReadFill()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            var b = new byte[22];
            stream.Read(b);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(22, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 22, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < b.Length; i++)
            {
                Assert.Equal(i % 256, b[i]);
            }
        }

        [Fact]
        public void ReadFill_Zero()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            var b = new byte[0];
            stream.Read(b);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(0, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Remaining);
            Assert.False(stream.Complete);
        }

        [Fact]
        public void ReadFill_OverEdge()
        {
            var streamLen = EdgeLocation * 2;
            var stream = GetStream(streamLen);
            var b = new byte[EdgeLocation + 15];
            var read = stream.Read(b);
            Assert.Equal(streamLen, stream.Length);
            Assert.Equal(b.Length, stream.Position);
            Assert.Equal(streamLen - b.Length, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < b.Length; i++)
            {
                Assert.Equal(i % 256, b[i]);
            }
        }

        [Fact]
        public void Read_Middle()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            var b = new byte[22];
            var read = stream.Read(b, 3, 10);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(10, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 10, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < 3; i++)
            {
                Assert.Equal(0, b[i]);
            }
            for (int i = 0; i < 10; i++)
            {
                Assert.Equal(i % 256, b[i + 3]);
            }
            for (int i = 13; i < b.Length; i++)
            {
                Assert.Equal(0, b[i]);
            }
        }

        [Fact]
        public void ReadZero()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            var b = new byte[22];
            var read = stream.Read(b, 3, 0);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(0, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < b.Length; i++)
            {
                Assert.Equal(0, b[i]);
            }
        }

        [Fact]
        public void Read_Full()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            var b = new byte[22];
            var read = stream.Read(b, 0, b.Length);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(b.Length, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - b.Length, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < b.Length; i++)
            {
                Assert.Equal(i % 256, b[i]);
            }
        }

        [Fact]
        public void Read_OverEdge()
        {
            var streamLen = EdgeLocation * 2;
            var stream = GetStream(streamLen);
            var b = new byte[EdgeLocation + 15];
            var read = stream.Read(b, 0, b.Length);
            Assert.Equal(streamLen, stream.Length);
            Assert.Equal(b.Length, stream.Position);
            Assert.Equal(streamLen - b.Length, stream.Remaining);
            Assert.False(stream.Complete);
            for (int i = 0; i < b.Length; i++)
            {
                Assert.Equal(i % 256, b[i]);
            }
        }

        [Fact]
        public void ReadByte()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadByte();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(4, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 4, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(3, result);
        }

        [Fact]
        public void ReadUInt16()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadUInt16();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(5, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 5, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(1027, result);
        }

        [Fact]
        public void ReadUInt32()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadUInt32();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(7, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 7, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal((uint)100_992_003, result);
        }

        [Fact]
        public void ReadUInt64()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadUInt64();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(11, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 11, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal((ulong)723_118_041_428_460_547L, result);
        }

        [Fact]
        public void ReadSByte()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadInt8();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(4, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 4, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(3, result);
        }

        [Fact]
        public void ReadInt16()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadInt16();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(5, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 5, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(1027, result);
        }

        [Fact]
        public void ReadInt32()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadInt32();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(7, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 7, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(100_992_003, result);
        }

        [Fact]
        public void ReadInt64()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadInt64();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(11, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 11, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(723_118_041_428_460_547L, result);
        }

        [Fact]
        public void ReadFloat()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadFloat();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(7, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 7, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(2.50174671E-35f, result);
        }

        [Fact]
        public void ReadDouble()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadDouble();
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(11, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 11, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(2.5437718470676246E-260d, result);
        }

        [Fact]
        public void ReadString()
        {
            var stream = GetStream(TYPICAL_TEST_LENGTH);
            stream.Position = 3;
            var result = stream.ReadString(5);
            Assert.Equal(5, result.Length);
            Assert.Equal(TYPICAL_TEST_LENGTH, stream.Length);
            Assert.Equal(8, stream.Position);
            Assert.Equal(TYPICAL_TEST_LENGTH - 8, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal("\u0003\u0004\u0005\u0006\u0007", result);
        }

        [Fact]
        public void ReadStringLargerThanBuffer()
        {
            var stream = GetStream(EdgeLocation * 2);
            stream.Position = 3;
            var result = stream.ReadString(EdgeLocation);
            Assert.Equal(EdgeLocation, result.Length);
            Assert.Equal(EdgeLocation * 2, stream.Length);
            Assert.Equal(EdgeLocation + 3, stream.Position);
            Assert.Equal(EdgeLocation - 3, stream.Remaining);
            Assert.False(stream.Complete);
            Assert.Equal(EdgeLocation, result.Length);
            Assert.Equal('\u0003', result[0]);
            Assert.Equal('f', result[result.Length - 1]);
        }

        [Fact]
        public void ReadLargeString_MoveBackPosition()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ReadLargeBytes_MoveBackPosition()
        {
            throw new NotImplementedException();
        }
    }
}
