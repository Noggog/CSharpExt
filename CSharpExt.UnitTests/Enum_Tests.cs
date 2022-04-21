using FluentAssertions;
using Xunit;

namespace CSharpExt.UnitTests;

public class Enum_Tests
{
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
}