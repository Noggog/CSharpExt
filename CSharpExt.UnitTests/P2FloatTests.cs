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
}