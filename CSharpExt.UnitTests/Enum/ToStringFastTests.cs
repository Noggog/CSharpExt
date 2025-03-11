using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class ToStringFastTests
{
    [Fact]
    public void ToStringFast()
    {
        TestEnum.Second.ToStringFast().ShouldBe(nameof(TestEnum.Second));
        ((TestEnum)17).ToStringFast().ShouldBe($"{17}");
        FlagsTestEnum.Four.ToStringFast().ShouldBe(nameof(FlagsTestEnum.Four));
        (FlagsTestEnum.Two | FlagsTestEnum.Four).ToStringFast().ShouldBe($"0x{6:x}");
        ((FlagsTestEnum)16).ToStringFast().ShouldBe($"0x{16:x}");
    }
    
    [Fact]
    public void ToStringFastLong()
    {
        LongEnum.Second.ToStringFast().ShouldBe(nameof(LongEnum.Second));
    }
}