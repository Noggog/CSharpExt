using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class TryGetEnumTypeTests
{
    [Fact]
    public void TypicalLookup()
    {
        Enums.TryGetEnumType("CSharpExt.UnitTests.Enum.TestEnum", out var type)
            .Should().BeTrue();
        type.Should().Be(typeof(TestEnum));
    }
    
    [Fact]
    public void TypicalFailedLookup()
    {
        Enums.TryGetEnumType("CSharpExt.UnitTests.Enum.TestEnum2", out var type)
            .Should().BeFalse();
    }
}