using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P2FloatTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2FloatParse(float x, float y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2Float(x, y);
        P2Float.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2FloatReparse(float x, float y)
    {
        var expectedPoint = new P2Float(x, y);
        P2Float.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void Dot_Product_Basic()
    {
        var a = new P2Float(1, 2);
        var b = new P2Float(3, 4);
        a.Dot(b).ShouldBe(1 * 3 + 2 * 4);
        P2Float.Dot(a, b).ShouldBe(1 * 3 + 2 * 4);
    }

    [Fact]
    public void Cross_Product_Basic()
    {
        var a = new P2Float(1, 2);
        var b = new P2Float(3, 4);
        a.Cross(b).ShouldBe(1 * 4 - 2 * 3);
    }

    [Fact]
    public void Dot_Product_Orthogonal()
    {
        var a = new P2Float(1, 0);
        var b = new P2Float(0, 1);
        a.Dot(b).ShouldBe(0);
    }

    [Fact]
    public void Cross_Product_Parallel()
    {
        var a = new P2Float(2, 2);
        var b = new P2Float(4, 4);
        a.Cross(b).ShouldBe(0);
    }

    [Fact]
    public void Dot_Product_Zero()
    {
        var a = new P2Float(0, 0);
        var b = new P2Float(5, -3);
        a.Dot(b).ShouldBe(0);
    }

    [Fact]
    public void Cross_Product_Zero()
    {
        var a = new P2Float(0, 0);
        var b = new P2Float(5, -3);
        a.Cross(b).ShouldBe(0);
    }

    [Fact]
    public void P2FloatParse_EmptyString_Fails()
    {
        P2Float.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2FloatParse_TooFewComponents_Fails()
    {
        P2Float.TryParse("1", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2FloatParse_TooManyComponents_Fails()
    {
        P2Float.TryParse("1,2,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2FloatParse_WithWhitespace_Succeeds()
    {
        P2Float.TryParse("1.5, 2.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Float(1.5f, 2.5f));
    }

    [Fact]
    public void P2FloatParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P2Float.TryParse(" 1.5 , 2.5 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Float(1.5f, 2.5f));
    }

    [Fact]
    public void P2FloatParse_NegativeNumbers_Succeeds()
    {
        P2Float.TryParse("-1.5,-2.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Float(-1.5f, -2.5f));
    }

    [Fact]
    public void P2FloatParse_InvalidFormat_Fails()
    {
        P2Float.TryParse("a,b", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2FloatParse_MixedInvalid_Fails()
    {
        P2Float.TryParse("1.5,b", out var result).ShouldBeFalse();
    }
}