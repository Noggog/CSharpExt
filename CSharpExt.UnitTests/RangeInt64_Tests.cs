using Noggog;
using Xunit;

namespace CSharpExt.UnitTests;

public class RangeInt64_Tests
{
    #region Collision
    [Fact]
    public void CollidesTypical()
    {
        Assert.True(
            new RangeInt64(5, 10).Collides(
                new RangeInt64(6, 11)));
    }

    [Fact]
    public void CollidesTypical_False_TooLow()
    {
        Assert.False(
            new RangeInt64(2, 3).Collides(
                new RangeInt64(6, 11)));
    }

    [Fact]
    public void CollidesTypical_False_TooHigh()
    {
        Assert.False(
            new RangeInt64(12, 15).Collides(
                new RangeInt64(6, 11)));
    }

    [Fact]
    public void CollidesTypical_Encapsulated()
    {
        Assert.True(
            new RangeInt64(7, 10).Collides(
                new RangeInt64(6, 11)));
    }

    [Fact]
    public void CollidesTypical_Encapsulating()
    {
        Assert.True(
            new RangeInt64(6, 11).Collides(
                new RangeInt64(7, 10)));
    }

    [Fact]
    public void CollidesTypical_Equal()
    {
        Assert.True(
            new RangeInt64(7, 10).Collides(
                new RangeInt64(7, 10)));
    }
    #endregion
}