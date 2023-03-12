using FluentAssertions;
using Noggog;

namespace CSharpExt.UnitTests.Enum;

public class IsDefinedTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Values.ForEach(x => Enums<TestEnum>.IsDefined(x).Should().BeTrue());
        Enums<EmptyTestEnum>.Values.ForEach(x => Enums<EmptyTestEnum>.IsDefined(x).Should().BeTrue());
        Enums<FlagsTestEnum>.Values.ForEach(x => Enums<FlagsTestEnum>.IsDefined(x).Should().BeTrue());
        Enums<EmptyFlagsTestEnum>.Values.ForEach(x => Enums<EmptyFlagsTestEnum>.IsDefined(x).Should().BeTrue());
    }
    
    [Fact]
    public void ByInt()
    {
        Enums<TestEnum>.Values.ForEach(x => Enums<TestEnum>.IsDefined((int)x).Should().BeTrue());
        Enums<EmptyTestEnum>.Values.ForEach(x => Enums<EmptyTestEnum>.IsDefined((int)x).Should().BeTrue());
        Enums<FlagsTestEnum>.Values.ForEach(x => Enums<FlagsTestEnum>.IsDefined((int)x).Should().BeTrue());
        Enums<EmptyFlagsTestEnum>.Values.ForEach(x => Enums<EmptyFlagsTestEnum>.IsDefined((int)x).Should().BeTrue());
    }
    
    [Fact]
    public void WrongType()
    {
        Assert.Throws<ArgumentException>(() => Enums<TestEnum>.IsDefined((long)15));
    }
    
    [Fact]
    public void NotDefined()
    {
        Enums<TestEnum>.IsDefined((TestEnum)115)
            .Should().BeFalse();
        Enums<FlagsTestEnum>.IsDefined((FlagsTestEnum)115)
            .Should().BeFalse();
    }
    
    [Fact]
    public void NotDefinedByInt()
    {
        Enums<TestEnum>.IsDefined(115)
            .Should().BeFalse();
        Enums<FlagsTestEnum>.IsDefined(115)
            .Should().BeFalse();
    }
}