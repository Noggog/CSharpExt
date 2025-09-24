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

    [Fact]
    public void P2IntParse_EmptyString_Fails()
    {
        P2Int.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2IntParse_TooFewComponents_Fails()
    {
        P2Int.TryParse("1", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2IntParse_TooManyComponents_Fails()
    {
        P2Int.TryParse("1,2,3", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2IntParse_WithWhitespace_Succeeds()
    {
        P2Int.TryParse("1, 2", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Int(1, 2));
    }

    [Fact]
    public void P2IntParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P2Int.TryParse(" 1 , 2 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Int(1, 2));
    }

    [Fact]
    public void P2IntParse_NegativeNumbers_Succeeds()
    {
        P2Int.TryParse("-1,-2", out var result).ShouldBeTrue();
        result.ShouldBe(new P2Int(-1, -2));
    }

    [Fact]
    public void P2IntParse_InvalidFormat_Fails()
    {
        P2Int.TryParse("a,b", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P2IntParse_MixedInvalid_Fails()
    {
        P2Int.TryParse("1,b", out var result).ShouldBeFalse();
    }
}