using Noggog;
using Noggog.Streams.Binary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSharpExt.Tests
{
    public class BasicSubstitutionRangeStream_Tests
    {
        public const int TypicalLength = 100;
        public const byte Substitution = 244;

        public byte[] GetBytes(int len)
        {
            byte[] ret = new byte[len];
            for (int i = 0; i < len; i++)
            {
                ret[i] = (byte)i;
            }
            return ret;
        }

        public MemoryStream GetStream(int len)
        {
            return new MemoryStream(
                GetBytes(len));
        }

        public MemoryStream GetTypicalMemStream()
        {
            return GetStream(TypicalLength);
        }

        public RangeCollection GetTypicalRangeCollection()
        {
            var rangeColl = new RangeCollection();
            rangeColl.Add(
                new RangeInt64(
                    4, 12));
            rangeColl.Add(
                new RangeInt64(
                    50, 60));
            return rangeColl;
        }

        public BasicSubstitutionRangeStream GetTypical()
        {
            return new BasicSubstitutionRangeStream(
                GetTypicalMemStream(),
                GetTypicalRangeCollection(),
                Substitution);
        }

        [Fact]
        public void ReadNoSubs()
        {
            var stream = new BasicSubstitutionRangeStream(
                GetTypicalMemStream(),
                subList: new Noggog.RangeCollection(),
                toSubstitute: 0);
            byte[] outBytes = new byte[TypicalLength];
            Assert.Equal(TypicalLength, stream.Read(outBytes, 0, TypicalLength));
            for (int i = 0; i < outBytes.Length; i++)
            {
                Assert.Equal((byte)i, outBytes[i]);
            }
        }

        [Fact]
        public void Read()
        {
            var rangeColl = GetTypicalRangeCollection();
            var stream = GetTypical();
            byte[] outBytes = new byte[TypicalLength];
            Assert.Equal(TypicalLength, stream.Read(outBytes, 0, TypicalLength));
            for (int i = 0; i < outBytes.Length; i++)
            {
                if (rangeColl.IsEncapsulated(i))
                {
                    Assert.Equal(Substitution, outBytes[i]);
                }
                else
                {
                    Assert.Equal((byte)i, outBytes[i]);
                }
            }
        }

        [Fact]
        public void ReadInRange()
        {
            var rangeColl = GetTypicalRangeCollection();
            var stream = GetTypical();
            int start = 5;
            stream.Position = start;
            byte[] outBytes = new byte[TypicalLength - start];
            Assert.Equal(TypicalLength - start, stream.Read(outBytes, 0, TypicalLength));
            for (int i = 0; i < outBytes.Length; i++)
            {
                if (rangeColl.IsEncapsulated(i + start))
                {
                    Assert.Equal(Substitution, outBytes[i]);
                }
                else
                {
                    Assert.Equal((byte)(i + start), outBytes[i]);
                }
            }
        }
    }
}
