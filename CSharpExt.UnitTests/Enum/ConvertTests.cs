using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class ConvertTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Convert(1).Should().Be(TestEnum.First);
        Enums<TestEnum>.Convert(2).Should().Be(TestEnum.Second);
        Enums<TestEnum>.Convert(150).Should().Be(TestEnum.Third);
    }
    
    [Fact]
    public void Flags()
    {
        Enums<FlagsTestEnum>.Convert(1).Should().Be(FlagsTestEnum.One);
        Enums<FlagsTestEnum>.Convert(2).Should().Be(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.Convert(4).Should().Be(FlagsTestEnum.Four);
    }
    
    [Fact]
    public void OutOfBounds()
    {
        Enums<TestEnum>.Convert(5).Should().Be((TestEnum)5);
    }
    
    [Fact]
    public void OutOfBoundsFlags()
    {
        Enums<FlagsTestEnum>.Convert(5).Should().Be((FlagsTestEnum)5);
    }
    
    [Fact]
    public void TryConvert()
    {
        Enums<TestEnum>.TryConvert(1, out var val).Should().BeTrue();
        val.Should().Be(TestEnum.First);
        Enums<TestEnum>.TryConvert(2, out val).Should().BeTrue();
        val.Should().Be(TestEnum.Second);
        Enums<TestEnum>.TryConvert(150, out val).Should().BeTrue();
        val.Should().Be(TestEnum.Third);
    }
    
    [Fact]
    public void TryConvertFlags()
    {
        Enums<FlagsTestEnum>.TryConvert(1, out var val).Should().BeTrue();
        val.Should().Be(FlagsTestEnum.One);
        Enums<FlagsTestEnum>.TryConvert(2, out val).Should().BeTrue();
        val.Should().Be(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.TryConvert(4, out val).Should().BeTrue();
        val.Should().Be(FlagsTestEnum.Four);
    }
}