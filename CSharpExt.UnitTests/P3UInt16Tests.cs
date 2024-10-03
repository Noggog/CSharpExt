using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P3UInt16Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3UInt16Parse(ushort x, ushort y, ushort z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3UInt16(x, y, z);
        P3UInt16.TryParse(givenStr, out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3UInt16Reparse(ushort x, ushort y, ushort z)
    {
        var expectedPoint = new P3UInt16(x, y, z);
        P3UInt16.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}