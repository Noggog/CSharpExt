using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P3FloatTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3FloatParse(float x, float y, float z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3Float(x, y, z);
        P3Float.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3FloatReparse(float x, float y, float z)
    {
        var expectedPoint = new P3Float(x, y, z);
        P3Float.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void Dot_Product_Basic()
    {
        var a = new P3Float(1, 2, 3);
        var b = new P3Float(4, 5, 6);
        a.Dot(b).ShouldBe(1 * 4 + 2 * 5 + 3 * 6);
        P3Float.Dot(a, b).ShouldBe(1 * 4 + 2 * 5 + 3 * 6);
    }

    [Fact]
    public void Cross_Product_Basic()
    {
        var a = new P3Float(1, 2, 3);
        var b = new P3Float(4, 5, 6);
        var expected = new P3Float(-3, 6, -3);
        a.Cross(b).ShouldBe(expected);
    }

    [Fact]
    public void Dot_Product_Orthogonal()
    {
        var a = new P3Float(1, 0, 0);
        var b = new P3Float(0, 1, 0);
        a.Dot(b).ShouldBe(0);
    }

    [Fact]
    public void Cross_Product_Parallel()
    {
        var a = new P3Float(2, 2, 2);
        var b = new P3Float(4, 4, 4);
        a.Cross(b).ShouldBe(new P3Float(0, 0, 0));
    }

    [Fact]
    public void Dot_Product_Zero()
    {
        var a = new P3Float(0, 0, 0);
        var b = new P3Float(5, -3, 7);
        a.Dot(b).ShouldBe(0);
    }

    [Fact]
    public void Cross_Product_Zero()
    {
        var a = new P3Float(0, 0, 0);
        var b = new P3Float(5, -3, 7);
        a.Cross(b).ShouldBe(new P3Float(0, 0, 0));
    }

    [Fact]
    public void P3FloatParse_EmptyString_Fails()
    {
        P3Float.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3FloatParse_TooFewComponents_Fails()
    {
        P3Float.TryParse("1,2", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3FloatParse_TooManyComponents_Fails()
    {
        P3Float.TryParse("1,2,3,4", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3FloatParse_WithWhitespace_Succeeds()
    {
        P3Float.TryParse("1.5, 2.5, 3.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Float(1.5f, 2.5f, 3.5f));
    }

    [Fact]
    public void P3FloatParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P3Float.TryParse(" 1.5 , 2.5 , 3.5 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Float(1.5f, 2.5f, 3.5f));
    }

    [Fact]
    public void P3FloatParse_NegativeNumbers_Succeeds()
    {
        P3Float.TryParse("-1.5,-2.5,-3.5", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Float(-1.5f, -2.5f, -3.5f));
    }

    [Fact]
    public void P3FloatParse_InvalidFormat_Fails()
    {
        P3Float.TryParse("a,b,c", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3FloatParse_MixedInvalid_Fails()
    {
        P3Float.TryParse("1.5,b,3.5", out var result).ShouldBeFalse();
    }
}