using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests;

public class RangeEnumerationTests
{
    // Take() bounds the read so a wrapping enumerator fails the test instead of hanging it.
    private const int Bound = 10;

    [Fact]
    public void UInt8EndingAtMaxValue() =>
        new RangeUInt8(byte.MaxValue - 2, byte.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new byte[] { 253, 254, 255 });

    [Fact]
    public void UInt8FullRange() =>
        new RangeUInt8(byte.MinValue, byte.MaxValue).Take(byte.MaxValue + 2).Count()
            .ShouldBe(byte.MaxValue + 1);

    [Fact]
    public void Int8EndingAtMaxValue() =>
        new RangeInt8(sbyte.MaxValue - 2, sbyte.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new sbyte[] { 125, 126, 127 });

    [Fact]
    public void Int16EndingAtMaxValue() =>
        new RangeInt16(short.MaxValue - 2, short.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new short[] { short.MaxValue - 2, short.MaxValue - 1, short.MaxValue });

    [Fact]
    public void UInt16EndingAtMaxValue() =>
        new RangeUInt16(ushort.MaxValue - 2, ushort.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new ushort[] { ushort.MaxValue - 2, ushort.MaxValue - 1, ushort.MaxValue });

    [Fact]
    public void Int32EndingAtMaxValue() =>
        new RangeInt32(int.MaxValue - 2, int.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new[] { int.MaxValue - 2, int.MaxValue - 1, int.MaxValue });

    [Fact]
    public void UInt32EndingAtMaxValue() =>
        new RangeUInt32(uint.MaxValue - 2, uint.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new[] { uint.MaxValue - 2, uint.MaxValue - 1, uint.MaxValue });

    [Fact]
    public void Int64EndingAtMaxValue() =>
        new RangeInt64(long.MaxValue - 2, long.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new[] { long.MaxValue - 2, long.MaxValue - 1, long.MaxValue });

    [Fact]
    public void UInt64EndingAtMaxValue() =>
        new RangeUInt64(ulong.MaxValue - 2, ulong.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new[] { ulong.MaxValue - 2, ulong.MaxValue - 1, ulong.MaxValue });

    [Fact]
    public void SingleValueAtMaxValue() =>
        new RangeUInt8(byte.MaxValue).Take(Bound).ToArray()
            .ShouldBe(new byte[] { byte.MaxValue });

    [Fact]
    public void Typical() =>
        new RangeInt32(3, 6).Take(Bound).ToArray()
            .ShouldBe(new[] { 3, 4, 5, 6 });

    [Fact]
    public void MinAboveMaxIsEmpty() =>
        new RangeUInt8 { Min = 6, Max = 3 }.Take(Bound).ToArray()
            .ShouldBeEmpty();
}
