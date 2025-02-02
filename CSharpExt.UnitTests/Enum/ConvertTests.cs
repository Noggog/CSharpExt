using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class ConvertTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.Convert(1).ShouldBe(TestEnum.First);
        Enums<TestEnum>.Convert(2).ShouldBe(TestEnum.Second);
        Enums<TestEnum>.Convert(150).ShouldBe(TestEnum.Third);
    }
    
    [Fact]
    public void Flags()
    {
        Enums<FlagsTestEnum>.Convert(1).ShouldBe(FlagsTestEnum.One);
        Enums<FlagsTestEnum>.Convert(2).ShouldBe(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.Convert(4).ShouldBe(FlagsTestEnum.Four);
    }
    
    [Fact]
    public void OutOfBounds()
    {
        Enums<TestEnum>.Convert(5).ShouldBe((TestEnum)5);
    }
    
    [Fact]
    public void OutOfBoundsFlags()
    {
        Enums<FlagsTestEnum>.Convert(5).ShouldBe((FlagsTestEnum)5);
    }
    
    [Fact]
    public void TryConvert()
    {
        Enums<TestEnum>.TryConvert(1, out var val).ShouldBeTrue();
        val.ShouldBe(TestEnum.First);
        Enums<TestEnum>.TryConvert(2, out val).ShouldBeTrue();
        val.ShouldBe(TestEnum.Second);
        Enums<TestEnum>.TryConvert(150, out val).ShouldBeTrue();
        val.ShouldBe(TestEnum.Third);
    }
    
    [Fact]
    public void TryConvertFlags()
    {
        Enums<FlagsTestEnum>.TryConvert(1, out var val).ShouldBeTrue();
        val.ShouldBe(FlagsTestEnum.One);
        Enums<FlagsTestEnum>.TryConvert(2, out val).ShouldBeTrue();
        val.ShouldBe(FlagsTestEnum.Two);
        Enums<FlagsTestEnum>.TryConvert(4, out val).ShouldBeTrue();
        val.ShouldBe(FlagsTestEnum.Four);
    }
    
    [Fact]
    public void IsLongEnum()
    {
        Enums<LongEnum>.Convert(2).ShouldBe((LongEnum)2);
        Enums<LongEnum>.Convert(long.MaxValue).ShouldBe((LongEnum)long.MaxValue);
        Enums<LongEnum>.Convert(long.MinValue).ShouldBe((LongEnum)long.MinValue);
    }
    
    [Fact]
    public void IsULongEnum()
    {
        Enums<ULongEnum>.Convert(2).ShouldBe((ULongEnum)2);
        Enums<ULongEnum>.Convert(long.MaxValue).ShouldBe((ULongEnum)long.MaxValue);
    }
    
    [Fact]
    public void IsByteEnum()
    {
        Enums<ByteEnum>.Convert(2).ShouldBe((ByteEnum)2);
    }
}