using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests;

public class Enum_Tests
{
    public enum Test
    {
        First,
        Second,
        Third,
    }
    
    [Flags]
    public enum TestFlags
    {
        One = 0x01,
        Two = 0x02,
        Four = 0x04,
    }

    [Fact]
    public void HasEnumFlags_NoApiConflicts()
    {
        TestFlags flags = TestFlags.One | TestFlags.Four;
        flags.HasFlag(TestFlags.Four)
            .Should().BeTrue();
    }

    [Fact]
    public void IsFlagsEnum()
    {
        EnumExt<Test>.IsFlags.Should().BeFalse();
        EnumExt<TestFlags>.IsFlags.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void EnumerateContainedFlags(bool includeUndefined)
    {
        EnumExt<Test>.EnumerateContainedFlags(Test.Second, includeUndefined: includeUndefined)
            .Should().Equal(Test.Second);
        EnumExt<TestFlags>.EnumerateContainedFlags(default(TestFlags), includeUndefined: includeUndefined)
            .Should().BeEmpty();
        EnumExt<TestFlags>.EnumerateContainedFlags(TestFlags.Two, includeUndefined: includeUndefined)
            .Should().Equal(TestFlags.Two);
        EnumExt<TestFlags>.EnumerateContainedFlags(TestFlags.Two | TestFlags.Four, includeUndefined: includeUndefined)
            .Should().Equal(TestFlags.Two, TestFlags.Four);
    }

    [Fact]
    public void EnumerateContainedFlagsUndefined()
    {
        EnumExt<Test>.EnumerateContainedFlags((Test)17, includeUndefined: false)
            .Should().BeEmpty();
        EnumExt<Test>.EnumerateContainedFlags((Test)17, includeUndefined: true)
            .Should().Equal((Test)17);
        EnumExt<TestFlags>.EnumerateContainedFlags(TestFlags.Two | TestFlags.Four | (TestFlags)16, includeUndefined: false)
            .Should().Equal(TestFlags.Two, TestFlags.Four);
        EnumExt<TestFlags>.EnumerateContainedFlags(TestFlags.Two | TestFlags.Four | (TestFlags)16, includeUndefined: true)
            .Should().Equal(TestFlags.Two, TestFlags.Four, (TestFlags)16);
    }
}