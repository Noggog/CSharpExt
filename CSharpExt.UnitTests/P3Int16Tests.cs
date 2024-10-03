using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P3Int16Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3Int16Parse(short x, short y, short z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3Int16(x, y, z);
        P3Int16.TryParse(givenStr, out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3Int16Reparse(short x, short y, short z)
    {
        var expectedPoint = new P3Int16(x, y, z);
        P3Int16.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}