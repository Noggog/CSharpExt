using FluentAssertions;
using Noggog;
using Xunit;

namespace CSharpExt.UnitTests.Enum;

public class TryGetNthTests
{
    [Fact]
    public void Typical()
    {
        Enums<TestEnum>.GetNth(1, TestEnum.Third)
            .Should().Be(TestEnum.Second);
        Enums<TestEnum>.TryGetNth(1, out var item)
            .Should().Be(true);
        item.Should().Be(TestEnum.Second);
        Enums.TryGetNth(typeof(TestEnum), 1, out var item2)
            .Should().Be(true);
        item2.Should().Be(TestEnum.Second);
    }
    
    [Fact]
    public void Negative()
    {
        Enums<TestEnum>.GetNth(-1, TestEnum.Third)
            .Should().Be(TestEnum.Third);
        Enums<TestEnum>.TryGetNth(-1, out var item)
            .Should().Be(false);
        Enums.TryGetNth(typeof(TestEnum), -1, out var item2)
            .Should().Be(false);
    }
    
    [Fact]
    public void Over()
    {
        Enums<TestEnum>.GetNth(17, TestEnum.Third)
            .Should().Be(TestEnum.Third);
        Enums<TestEnum>.TryGetNth(17, out var item)
            .Should().Be(false);
        Enums.TryGetNth(typeof(TestEnum), 17, out var item2)
            .Should().Be(false);
    }
}