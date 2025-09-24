using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P3UInt16Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3UInt16Parse(ushort x, ushort y, ushort z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3UInt16(x, y, z);
        P3UInt16.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3UInt16Reparse(ushort x, ushort y, ushort z)
    {
        var expectedPoint = new P3UInt16(x, y, z);
        P3UInt16.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P3UInt16Parse_EmptyString_Fails()
    {
        P3UInt16.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt16Parse_TooFewComponents_Fails()
    {
        P3UInt16.TryParse("1,2", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt16Parse_TooManyComponents_Fails()
    {
        P3UInt16.TryParse("1,2,3,4", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt16Parse_WithWhitespace_Succeeds()
    {
        P3UInt16.TryParse("1, 2, 3", out var result).ShouldBeTrue();
        result.ShouldBe(new P3UInt16(1, 2, 3));
    }

    [Fact]
    public void P3UInt16Parse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P3UInt16.TryParse(" 1 , 2 , 3 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P3UInt16(1, 2, 3));
    }

    [Fact]
    public void P3UInt16Parse_InvalidFormat_Fails()
    {
        P3UInt16.TryParse("a,b,c", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt16Parse_MixedInvalid_Fails()
    {
        P3UInt16.TryParse("1,b,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3UInt16Parse_NegativeNumber_Fails()
    {
        P3UInt16.TryParse("-1,2,3", out var result).ShouldBeFalse();
    }
}