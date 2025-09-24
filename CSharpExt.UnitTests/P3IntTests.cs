using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests;

public class P3IntTests
{
    [Theory]
    [DefaultAutoData]
    public void TypicalP3IntParse(int x, int y, int z)
    {
        var givenStr = $"{x},{y},{z}";
        var expectedPoint = new P3Int(x, y, z);
        P3Int.TryParse(givenStr, out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }
    
    [Theory]
    [DefaultAutoData]
    public void P3IntReparse(int x, int y, int z)
    {
        var expectedPoint = new P3Int(x, y, z);
        P3Int.TryParse(expectedPoint.ToString(), out var result).ShouldBeTrue();
        result.ShouldBe(expectedPoint);
    }

    [Fact]
    public void P3IntParse_EmptyString_Fails()
    {
        P3Int.TryParse("", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3IntParse_TooFewComponents_Fails()
    {
        P3Int.TryParse("1,2", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3IntParse_TooManyComponents_Fails()
    {
        P3Int.TryParse("1,2,3,4", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3IntParse_WithWhitespace_Succeeds()
    {
        P3Int.TryParse("1, 2, 3", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Int(1, 2, 3));
    }

    [Fact]
    public void P3IntParse_WithLeadingTrailingWhitespace_Succeeds()
    {
        P3Int.TryParse(" 1 , 2 , 3 ", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Int(1, 2, 3));
    }

    [Fact]
    public void P3IntParse_NegativeNumbers_Succeeds()
    {
        P3Int.TryParse("-1,-2,-3", out var result).ShouldBeTrue();
        result.ShouldBe(new P3Int(-1, -2, -3));
    }

    [Fact]
    public void P3IntParse_InvalidFormat_Fails()
    {
        P3Int.TryParse("a,b,c", out var result).ShouldBeFalse();
    }

    [Fact]
    public void P3IntParse_MixedInvalid_Fails()
    {
        P3Int.TryParse("1,b,3", out var result).ShouldBeFalse();
    }
}