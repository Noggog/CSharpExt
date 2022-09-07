using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class NumEntriesTests
{
    [Fact]
    public void EmptyEnum()
    {
        var e = default(EmptyTestEnum);
        e.NumEntries().Should().Be(0);
        Enums<EmptyTestEnum>.NumEntries.Should().Be(0);
    }
    
    [Fact]
    public void EmptyFlagsEnum()
    {
        var e = default(EmptyFlagsTestEnum);
        e.NumEntries().Should().Be(0);
        Enums<EmptyFlagsTestEnum>.NumEntries.Should().Be(0);
    }
    
    [Fact]
    public void Enum()
    {
        var e = default(TestEnum);
        e.NumEntries().Should().Be(3);
        Enums<TestEnum>.NumEntries.Should().Be(3);
    }
    
    [Fact]
    public void FlagsEnum()
    {
        var e = default(FlagsTestEnum);
        e.NumEntries().Should().Be(3);
        Enums<FlagsTestEnum>.NumEntries.Should().Be(3);
    }
}