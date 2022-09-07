using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class FlagTests
{
    [Fact]
    public void HasEnumFlags_NoApiConflicts()
    {
        var e = FlagsTestEnum.One | FlagsTestEnum.Four;
        e.HasFlag(FlagsTestEnum.Four)
            .Should().BeTrue();
    }
    [Fact]
    public void HasFlags()
    {
        var e = FlagsTestEnum.One | FlagsTestEnum.Four;
        e.HasFlag(FlagsTestEnum.Four)
            .Should().BeTrue();
    }
    
    private void HasFlagTest(byte val, byte test, bool result)
    {
        Enums.HasFlag((int)val, (int)test).Should().Be(result);
        Enums.HasFlag((uint)val, (uint)test).Should().Be(result);
        Enums.HasFlag((byte)val, (byte)test).Should().Be(result);
    }
    
    [Fact]
    public void HasFlag()
    {
        HasFlagTest(0x8, 0x8, true);
        HasFlagTest(0x8, 0x0, false);
        HasFlagTest(0x8, 0x10, false);
        HasFlagTest(0x18, 0x10, true);
        HasFlagTest(0x0, 0x10, false);
    }
    
    private void SetFlagTest(byte val, byte test, bool on, byte result)
    {
        Enums.SetFlag((int)val, (int)test, on).Should().Be(result);
        Enums.SetFlag((uint)val, (uint)test, on).Should().Be(result);
        Enums.SetFlag((byte)val, (byte)test, on).Should().Be(result);
        byte b = val;
        Enums.SetFlag(ref b, test, on);
        b.Should().Be(result);
        uint ui = val;
        Enums.SetFlag(ref ui, test, on);
        ui.Should().Be(result);
        int i = val;
        Enums.SetFlag(ref i, test, on);
        i.Should().Be(result);
    }
    
    [Fact]
    public void SetFlag()
    {
        SetFlagTest(0, 0x8, true, 0x8);
        SetFlagTest(0, 0x8, false, 0x0);
        SetFlagTest(0x10, 0x8, true, 0x18);
        SetFlagTest(0x18, 0x8, false, 0x10);
        SetFlagTest(0x10, 0x0, true, 0x10);
        SetFlagTest(0x18, 0x0, false, 0x18);
    }

    [Fact]
    public void IsFlagsEnum()
    {
        Enums<TestEnum>.IsFlagsEnum.Should().BeFalse();
        Enums<FlagsTestEnum>.IsFlagsEnum.Should().BeTrue();
    }
}