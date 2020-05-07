using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noggog;
using Xunit;

namespace CSharpExt.Tests
{
    public class BinaryReadStream_Tests : IBinaryStream_Tests
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
    }
}
