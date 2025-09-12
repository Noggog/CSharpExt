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
}