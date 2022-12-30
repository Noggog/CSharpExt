using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class EnumerateContainedFlagsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedFlags(bool includeUndefined)
    {
        Enums<TestEnum>.EnumerateContainedFlags(TestEnum.Second, includeUndefined: includeUndefined)
            .Should().Equal(TestEnum.Second);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(default(FlagsTestEnum), includeUndefined: includeUndefined)
            .Should().BeEmpty();
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two, includeUndefined: includeUndefined)
            .Should().Equal(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four, includeUndefined: includeUndefined)
            .Should().Equal(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }

    [Fact]
    public void EnumerateContainedFlagsUndefined()
    {
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: false)
            .Should().BeEmpty();
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: true)
            .Should().Equal((TestEnum)17);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: false)
            .Should().Equal(FlagsTestEnum.Two, FlagsTestEnum.Four);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: true)
            .Should().Equal(FlagsTestEnum.Two, FlagsTestEnum.Four, (FlagsTestEnum)16);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)(-16), includeUndefined: false)
            .Should().Equal(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedUlongFlags(bool includeUndefined)
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(default(UlongFlagsTestEnum), includeUndefined: includeUndefined)
            .Should().BeEmpty();
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .Should().Equal(UlongFlagsTestEnum.Two);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .Should().Equal(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .Should().Equal(UlongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedUlongFlagsUndefined()
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: false)
            .Should().Equal(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: true)
            .Should().Equal(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four, (UlongFlagsTestEnum)16);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: true)
            .Should().Equal(UlongFlagsTestEnum.Max);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedLongFlags(bool includeUndefined)
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(default(LongFlagsTestEnum), includeUndefined: includeUndefined)
            .Should().BeEmpty();
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .Should().Equal(LongFlagsTestEnum.Two);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .Should().Equal(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .Should().Equal(LongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedLongFlagsUndefined()
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: false)
            .Should().Equal(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: true)
            .Should().Equal(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four, (LongFlagsTestEnum)16);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: false)
            .Should().Equal(LongFlagsTestEnum.Max);
    }
}