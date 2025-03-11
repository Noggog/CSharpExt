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
}