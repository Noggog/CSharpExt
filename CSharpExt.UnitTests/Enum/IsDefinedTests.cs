using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class IsDefinedTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Values.ForEach(x => Enums<TestEnum>.IsDefined(x).ShouldBeTrue());
        Enums<EmptyTestEnum>.Values.ForEach(x => Enums<EmptyTestEnum>.IsDefined(x).ShouldBeTrue());
        Enums<FlagsTestEnum>.Values.ForEach(x => Enums<FlagsTestEnum>.IsDefined(x).ShouldBeTrue());
        Enums<EmptyFlagsTestEnum>.Values.ForEach(x => Enums<EmptyFlagsTestEnum>.IsDefined(x).ShouldBeTrue());
    }
    
    [Fact]
    public void ByInt()
    {
        Enums<TestEnum>.Values.ForEach(x => Enums<TestEnum>.IsDefined((int)x).ShouldBeTrue());
        Enums<EmptyTestEnum>.Values.ForEach(x => Enums<EmptyTestEnum>.IsDefined((int)x).ShouldBeTrue());
        Enums<FlagsTestEnum>.Values.ForEach(x => Enums<FlagsTestEnum>.IsDefined((int)x).ShouldBeTrue());
        Enums<EmptyFlagsTestEnum>.Values.ForEach(x => Enums<EmptyFlagsTestEnum>.IsDefined((int)x).ShouldBeTrue());
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
            .ShouldBeFalse();
        Enums<FlagsTestEnum>.IsDefined((FlagsTestEnum)115)
            .ShouldBeFalse();
    }
    
    [Fact]
    public void NotDefinedByInt()
    {
        Enums<TestEnum>.IsDefined(115)
            .ShouldBeFalse();
        Enums<FlagsTestEnum>.IsDefined(115)
            .ShouldBeFalse();
    }
}