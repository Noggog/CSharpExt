using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P2UInt8Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2UInt8Parse(byte x, byte y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2UInt8(x, y);
        P2UInt8.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2UInt8Reparse(byte x, byte y)
    {
        var expectedPoint = new P2UInt8(x, y);
        P2UInt8.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P2UInt8Parse_EmptyString_Fails()
    {
        P2UInt8.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2UInt8Parse_TooFewComponents_Fails()
    {
        P2UInt8.TryParse("1", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2UInt8Parse_TooManyComponents_Fails()
    {
        P2UInt8.TryParse("1,2,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2UInt8Parse_WithWhitespace_Succeeds()
    {
        P2UInt8.TryParse("1, 2", out var result).ShouldBeTrue();
        result.ShouldBe(new P2UInt8(1, 2));
    }

    [Fact]
    public void P2UInt8Parse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P2UInt8.TryParse(" 1 , 2 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P2UInt8(1, 2));
    }

    [Fact]
    public void P2UInt8Parse_InvalidFormat_Fails()
    {
        P2UInt8.TryParse("a,b", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2UInt8Parse_MixedInvalid_Fails()
    {
        P2UInt8.TryParse("1,b", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2UInt8Parse_NegativeNumber_Fails()
    {
        P2UInt8.TryParse("-1,2", out var result).ShouldBeFalse();
    }
}