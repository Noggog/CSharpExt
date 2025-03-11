using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class TryGetEnumTypeTests
{
    [Fact]
    public void TypicalLookup()
    {
        Enums.TryGetEnumType("CSharpExt.UnitTests.Enum.TestEnum", out var type)
            .ShouldBeTrue();
        type.ShouldBe(typeof(TestEnum));
    }
    
    [Fact]
    public void TypicalFailedLookup()
    {
        Enums.TryGetEnumType("CSharpExt.UnitTests.Enum.TestEnum2", out var type)
            .ShouldBeFalse();
    }
}