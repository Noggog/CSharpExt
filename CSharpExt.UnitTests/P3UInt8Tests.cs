using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P3UInt8Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3UInt8Parse(byte x, byte y, byte z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3UInt8(x, y, z);
        P3UInt8.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3UInt8Reparse(byte x, byte y, byte z)
    {
        var expectedPoint = new P3UInt8(x, y, z);
        P3UInt8.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P3UInt8Parse_EmptyString_Fails()
    {
        P3UInt8.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt8Parse_TooFewComponents_Fails()
    {
        P3UInt8.TryParse("1,2", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt8Parse_TooManyComponents_Fails()
    {
        P3UInt8.TryParse("1,2,3,4", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt8Parse_WithWhitespace_Succeeds()
    {
        P3UInt8.TryParse("1, 2, 3", out var result).ShouldBeTrue();
        result.ShouldBe(new P3UInt8(1, 2, 3));
    }

    [Fact]
    public void P3UInt8Parse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P3UInt8.TryParse(" 1 , 2 , 3 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P3UInt8(1, 2, 3));
    }

    [Fact]
    public void P3UInt8Parse_InvalidFormat_Fails()
    {
        P3UInt8.TryParse("a,b,c", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt8Parse_MixedInvalid_Fails()
    {
        P3UInt8.TryParse("1,b,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt8Parse_NegativeNumber_Fails()
    {
        P3UInt8.TryParse("-1,2,3", out var result).ShouldBeFalse();
    }
}