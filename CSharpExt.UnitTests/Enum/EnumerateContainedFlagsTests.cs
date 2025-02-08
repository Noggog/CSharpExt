using Noggog;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class EnumerateContainedFlagsTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedFlags(bool includeUndefined)
    {
        Enums<TestEnum>.EnumerateContainedFlags(TestEnum.Second, includeUndefined: includeUndefined)
            .ShouldEqual(TestEnum.Second);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(default(FlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqual(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqual(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }

    [Fact]
    public void EnumerateContainedFlagsUndefined()
    {
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: false)
            .ShouldBeEmpty();
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: true)
            .ShouldEqual((TestEnum)17);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: false)
            .ShouldEqual(FlagsTestEnum.Two, FlagsTestEnum.Four);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: true)
            .ShouldEqual(FlagsTestEnum.Two, FlagsTestEnum.Four, (FlagsTestEnum)16);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)(-16), includeUndefined: false)
            .ShouldEqual(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedUlongFlags(bool includeUndefined)
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(default(UlongFlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqual(UlongFlagsTestEnum.Two);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqual(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .ShouldEqual(UlongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedUlongFlagsUndefined()
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: false)
            .ShouldEqual(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: true)
            .ShouldEqual(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four, (UlongFlagsTestEnum)16);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: true)
            .ShouldEqual(UlongFlagsTestEnum.Max);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedLongFlags(bool includeUndefined)
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(default(LongFlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqual(LongFlagsTestEnum.Two);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqual(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .ShouldEqual(LongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedLongFlagsUndefined()
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: false)
            .ShouldEqual(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: true)
            .ShouldEqual(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four, (LongFlagsTestEnum)16);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: false)
            .ShouldEqual(LongFlagsTestEnum.Max);
    }
}