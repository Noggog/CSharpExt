using Noggog;
using Noggog.Testing.IO;
using Shouldly;

namespace CSharpExt.UnitTests;

public class StreamExtTests
{
    public class ContentsEqual
    {
        [Fact]
        public void Typical()
        {
            var b1 = Enumerable.Range(0, 1000).Select(x => (byte)x).ToArray();
            var mem1 = new MemoryStream(b1);
            var mem2 = new MemoryStream(b1);
            mem1.ContentsEqual(mem2).ShouldBeTrue();
        }
            
        [Fact]
        public void LengthDiff()
        {
            var b1 = Enumerable.Range(0, 1000).Select(x => (byte)x).ToArray();
            var b2 = Enumerable.Range(0, 950).Select(x => (byte)x).ToArray();
            var mem1 = new MemoryStream(b1);
            var mem2 = new MemoryStream(b2);
            mem1.ContentsEqual(mem2).ShouldBeFalse();
        }
            
        [Fact]
        public void ContentDiff()
        {
            var b1 = Enumerable.Range(0, 1000).Select(x => (byte)x).ToArray();
            var b2 = Enumerable.Range(0, 1000).Select(x => (byte)(x + 1)).ToArray();
            var mem1 = new MemoryStream(b1);
            var mem2 = new MemoryStream(b2);
            mem1.ContentsEqual(mem2).ShouldBeFalse();
        }
            
        [Fact]
        public void LengthLiar()
        {
            var b1 = Enumerable.Range(0, 1000).Select(_ => (byte)0).ToArray();
            var b2 = Enumerable.Range(0, 950).Select(_ => (byte)0).ToArray();
            var mem1 = new MemoryStream(b1);
            var mem2 = new LengthLiarStream(new MemoryStream(b2), 1000);
            mem1.ContentsEqual(mem2).ShouldBeFalse();
        }
    }
}