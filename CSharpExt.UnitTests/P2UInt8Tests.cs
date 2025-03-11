using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P2UInt8Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2UInt8Parse(byte x, byte y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2UInt8(x, y);
        P2UInt8.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2UInt8Reparse(byte x, byte y)
    {
        var expectedPoint = new P2UInt8(x, y);
        P2UInt8.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
}