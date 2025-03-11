using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class FlagTests
{
    [Fact]
    public void HasEnumFlags_NoApiConflicts()
    {
        var e = FlagsTestEnum.One | FlagsTestEnum.Four;
        e.HasFlag(FlagsTestEnum.Four)
            .ShouldBeTrue();
    }
    
    [Fact]
    public void HasFlags()
    {
        var e = FlagsTestEnum.One | FlagsTestEnum.Four;
        e.HasFlag(FlagsTestEnum.Four)
            .ShouldBeTrue();
    }
    
    [Fact]
    public void HasMultipleFlags()
    {
        var e = FlagsTestEnum.One | FlagsTestEnum.Four;
        e.HasFlag(FlagsTestEnum.Four | FlagsTestEnum.One)
            .ShouldBeTrue();
        e.HasFlag(FlagsTestEnum.Four)
            .ShouldBeTrue();
        e.HasFlag(FlagsTestEnum.One)
            .ShouldBeTrue();
        e.HasFlag(FlagsTestEnum.One | FlagsTestEnum.Two)
            .ShouldBeFalse();
    }
    
    private void HasFlagTest(byte val, byte test, bool result)
    {
        Enums.HasFlag((int)val, (int)test).ShouldBe(result);
        Enums.HasFlag((uint)val, (uint)test).ShouldBe(result);
        Enums.HasFlag((byte)val, (byte)test).ShouldBe(result);
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
        Enums.SetFlag((int)val, (int)test, on).ShouldBe(result);
        Enums.SetFlag((uint)val, (uint)test, on).ShouldBe(result);
        Enums.SetFlag((byte)val, (byte)test, on).ShouldBe(result);
        byte b = val;
        Enums.SetFlag(ref b, test, on);
        b.ShouldBe(result);
        uint ui = val;
        Enums.SetFlag(ref ui, test, on);
        ui.ShouldBe(result);
        int i = val;
        Enums.SetFlag(ref i, test, on);
        i.ShouldBe(result);
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
        Enums<TestEnum>.IsFlagsEnum.ShouldBeFalse();
        Enums<FlagsTestEnum>.IsFlagsEnum.ShouldBeTrue();
    }
}