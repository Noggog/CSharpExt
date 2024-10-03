using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P3UInt8Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3UInt8Parse(byte x, byte y, byte z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3UInt8(x, y, z);
        P3UInt8.TryParse(givenStr, out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3UInt8Reparse(byte x, byte y, byte z)
    {
        var expectedPoint = new P3UInt8(x, y, z);
        P3UInt8.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}