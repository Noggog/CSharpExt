using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class NumEntriesTests
{
    [Fact]
    public void EmptyEnum()
    {
        var e = default(EmptyTestEnum);
        e.NumEntries().ShouldBe(0);
        Enums<EmptyTestEnum>.NumEntries.ShouldBe(0);
    }
    
    [Fact]
    public void EmptyFlagsEnum()
    {
        var e = default(EmptyFlagsTestEnum);
        e.NumEntries().ShouldBe(0);
        Enums<EmptyFlagsTestEnum>.NumEntries.ShouldBe(0);
    }
    
    [Fact]
    public void Enum()
    {
        var e = default(TestEnum);
        e.NumEntries().ShouldBe(3);
        Enums<TestEnum>.NumEntries.ShouldBe(3);
    }
    
    [Fact]
    public void FlagsEnum()
    {
        var e = default(FlagsTestEnum);
        e.NumEntries().ShouldBe(3);
        Enums<FlagsTestEnum>.NumEntries.ShouldBe(3);
    }
}