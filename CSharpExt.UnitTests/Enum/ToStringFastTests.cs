using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class ToStringFastTests
{
    [Fact]
    public void ToStringFast()
    {
        TestEnum.Second.ToStringFast().Should().Be(nameof(TestEnum.Second));
        ((TestEnum)17).ToStringFast().Should().Be($"{17}");
        FlagsTestEnum.Four.ToStringFast().Should().Be(nameof(FlagsTestEnum.Four));
        (FlagsTestEnum.Two | FlagsTestEnum.Four).ToStringFast().Should().Be($"0x{6:x}");
        ((FlagsTestEnum)16).ToStringFast().Should().Be($"0x{16:x}");
    }
    
    [Fact]
    public void ToStringFastLong()
    {
        LongEnum.Second.ToStringFast().Should().Be(nameof(LongEnum.Second));
    }
}