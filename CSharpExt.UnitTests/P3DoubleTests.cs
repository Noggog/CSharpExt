using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P3DoubleTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3DoubleParse(double x, double y, double z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3Double(x, y, z);
        P3Double.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3DoubleReparse(double x, double y, double z)
    {
        var expectedPoint = new P3Double(x, y, z);
        P3Double.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P3DoubleParse_EmptyString_Fails()
    {
        P3Double.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3DoubleParse_TooFewComponents_Fails()
    {
        P3Double.TryParse("1,2", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3DoubleParse_TooManyComponents_Fails()
    {
        P3Double.TryParse("1,2,3,4", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3DoubleParse_WithWhitespace_Succeeds()
    {
        P3Double.TryParse("1.5, 2.5, 3.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Double(1.5, 2.5, 3.5));
    }

    [Fact]
    public void P3DoubleParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P3Double.TryParse(" 1.5 , 2.5 , 3.5 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Double(1.5, 2.5, 3.5));
    }

    [Fact]
    public void P3DoubleParse_NegativeNumbers_Succeeds()
    {
        P3Double.TryParse("-1.5,-2.5,-3.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Double(-1.5, -2.5, -3.5));
    }

    [Fact]
    public void P3DoubleParse_InvalidFormat_Fails()
    {
        P3Double.TryParse("a,b,c", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3DoubleParse_MixedInvalid_Fails()
    {
        P3Double.TryParse("1.5,b,3.5", out var result).ShouldBeFalse();
    }
}