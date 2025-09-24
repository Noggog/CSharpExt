using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P2DoubleTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2DoubleParse(double x, double y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2Double(x, y);
        P2Double.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2DoubleReparse(double x, double y)
    {
        var expectedPoint = new P2Double(x, y);
        P2Double.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P2DoubleParse_EmptyString_Fails()
    {
        P2Double.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2DoubleParse_TooFewComponents_Fails()
    {
        P2Double.TryParse("1", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2DoubleParse_TooManyComponents_Fails()
    {
        P2Double.TryParse("1,2,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2DoubleParse_WithWhitespace_Succeeds()
    {
        P2Double.TryParse("1.5, 2.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Double(1.5, 2.5));
    }

    [Fact]
    public void P2DoubleParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P2Double.TryParse(" 1.5 , 2.5 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Double(1.5, 2.5));
    }

    [Fact]
    public void P2DoubleParse_NegativeNumbers_Succeeds()
    {
        P2Double.TryParse("-1.5,-2.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Double(-1.5, -2.5));
    }

    [Fact]
    public void P2DoubleParse_InvalidFormat_Fails()
    {
        P2Double.TryParse("a,b", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2DoubleParse_MixedInvalid_Fails()
    {
        P2Double.TryParse("1.5,b", out var result).ShouldBeFalse();
    }
}