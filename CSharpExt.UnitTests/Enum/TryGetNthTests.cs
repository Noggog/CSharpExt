using Noggog;
using Shouldly;

namespace CSharpExt.UnitTests.Enum;

public class TryGetNthTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.GetNth(1, TestEnum.Third)
            .ShouldBe(TestEnum.Second);
        Enums<TestEnum>.TryGetNth(1, out var item)
            .ShouldBe(true);
        item.ShouldBe(TestEnum.Second);
        Enums.TryGetNth(typeof(TestEnum), 1, out var item2)
            .ShouldBe(true);
        item2.ShouldBe(TestEnum.Second);
    }
    
    [Fact]
    public void Negative()
    {
        Enums<TestEnum>.GetNth(-1, TestEnum.Third)
            .ShouldBe(TestEnum.Third);
        Enums<TestEnum>.TryGetNth(-1, out var item)
            .ShouldBe(false);
        Enums.TryGetNth(typeof(TestEnum), -1, out var item2)
            .ShouldBe(false);
    }
    
    [Fact]
    public void Over()
    {
        Enums<TestEnum>.GetNth(17, TestEnum.Third)
            .ShouldBe(TestEnum.Third);
        Enums<TestEnum>.TryGetNth(17, out var item)
            .ShouldBe(false);
        Enums.TryGetNth(typeof(TestEnum), 17, out var item2)
            .ShouldBe(false);
    }
}