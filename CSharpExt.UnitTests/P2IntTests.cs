using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P2IntTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2IntParse(int x, int y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2Int(x, y);
        P2Int.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2IntReparse(int x, int y)
    {
        var expectedPoint = new P2Int(x, y);
        P2Int.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
}