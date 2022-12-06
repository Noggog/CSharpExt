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
}