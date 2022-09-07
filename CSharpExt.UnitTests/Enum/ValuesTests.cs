using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class ValuesTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Values.Should().Equal(
            TestEnum.First,
            TestEnum.Second,
            TestEnum.Third);
    }

    [Fact]
    public void Empty()
    {
        Enums<EmptyTestEnum>.Values.Should().BeEmpty();
    }

    [Fact]
    public void TypicalFlags()
    {
        Enums<FlagsTestEnum>.Values.Should().Equal(
            FlagsTestEnum.One,
            FlagsTestEnum.Two,
            FlagsTestEnum.Four);
    }

    [Fact]
    public void EmptyFlags()
    {
        Enums<EmptyFlagsTestEnum>.Values.Should().BeEmpty();
    }
}