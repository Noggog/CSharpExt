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
}