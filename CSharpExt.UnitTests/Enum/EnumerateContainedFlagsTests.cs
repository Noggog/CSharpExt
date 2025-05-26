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
            .ShouldEqualEnumerable(TestEnum.Second);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(default(FlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }

    [Fact]
    public void EnumerateContainedFlagsUndefined()
    {
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: false)
            .ShouldBeEmpty();
        Enums<TestEnum>.EnumerateContainedFlags((TestEnum)17, includeUndefined: true)
            .ShouldEqualEnumerable((TestEnum)17);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: false)
            .ShouldEqualEnumerable(FlagsTestEnum.Two, FlagsTestEnum.Four);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)16, includeUndefined: true)
            .ShouldEqualEnumerable(FlagsTestEnum.Two, FlagsTestEnum.Four, (FlagsTestEnum)16);
        Enums<FlagsTestEnum>.EnumerateContainedFlags(FlagsTestEnum.Two | FlagsTestEnum.Four | (FlagsTestEnum)(-16), includeUndefined: false)
            .ShouldEqualEnumerable(FlagsTestEnum.Two, FlagsTestEnum.Four);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedUlongFlags(bool includeUndefined)
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(default(UlongFlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Two);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedUlongFlagsUndefined()
    {
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: false)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Two | UlongFlagsTestEnum.Four | (UlongFlagsTestEnum)16, includeUndefined: true)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Two, UlongFlagsTestEnum.Four, (UlongFlagsTestEnum)16);
        Enums<UlongFlagsTestEnum>.EnumerateContainedFlags(UlongFlagsTestEnum.Max, includeUndefined: true)
            .ShouldEqualEnumerable(UlongFlagsTestEnum.Max);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedLongFlags(bool includeUndefined)
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(default(LongFlagsTestEnum), includeUndefined: includeUndefined)
            .ShouldBeEmpty();
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Two);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: includeUndefined)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Max);
    }

    [Fact]
    public void EnumerateContainedLongFlagsUndefined()
    {
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: false)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Two | LongFlagsTestEnum.Four | (LongFlagsTestEnum)16, includeUndefined: true)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Two, LongFlagsTestEnum.Four, (LongFlagsTestEnum)16);
        Enums<LongFlagsTestEnum>.EnumerateContainedFlags(LongFlagsTestEnum.Max, includeUndefined: false)
            .ShouldEqualEnumerable(LongFlagsTestEnum.Max);
    }
}