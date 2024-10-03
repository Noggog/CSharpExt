using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests;

public class P2Int16Tests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP2Int16Parse(short x, short y)
    {
        var givenStr = $"{x},{y}";
        var expectedPoint = new P2Int16(x, y);
        var expectedBool = true;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void MaxAndMin()
    {
        var givenStr = $"{short.MinValue},{short.MaxValue}";
        var expectedPoint = new P2Int16(short.MinValue, short.MaxValue);
        var expectedBool = true;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void HelloWorld()
    {
        var givenStr = "Hello World";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void Empty()
    {
        var givenStr = "";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void NoComma()
    {
        var givenStr = "100";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void OneNumber()
    {
        var givenStr = "100,";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void ThreeNumbers()
    {
        var givenStr = "100,879,222";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }

    [Fact]
    public void NumbersBiggerThanShortMax()
    {
        var givenStr = "54687643846846,48979878979";
        var expectedPoint = default(P2Int16);
        var expectedBool = false;
        P2Int16.TryParse(givenStr, out var result).Should().Be(expectedBool);
        result.Should().Be(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P2Int16Reparse(short x, short y)
    {
        var expectedPoint = new P2Int16(x, y);
        P2Int16.TryParse(expectedPoint.ToString(), out var result).Should().BeTrue();
        result.Should().Be(expectedPoint);
    }
}