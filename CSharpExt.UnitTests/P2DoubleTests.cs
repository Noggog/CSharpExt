using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P2DoubleTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2DoubleParse(double x, double y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2Double(x, y);
        P2Double.TryParse(givenStr, out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2DoubleReparse(double x, double y)
    {
        var expectedPoint = new P2Double(x, y);
        P2Double.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}