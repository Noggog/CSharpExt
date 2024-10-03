using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P3IntTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3IntParse(int x, int y, int z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3Int(x, y, z);
        P3Int.TryParse(givenStr, out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3IntReparse(int x, int y, int z)
    {
        var expectedPoint = new P3Int(x, y, z);
        P3Int.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}