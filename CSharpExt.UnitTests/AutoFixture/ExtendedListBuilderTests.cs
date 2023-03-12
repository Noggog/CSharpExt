using FluentAssertions;
using Noggog;
using Noggog.Testing.AutoFixture;

namespace CSharpExt.UnitTests.AutoFixture;

public class ExtendedListBuilderTests
{
    [Theory, DefaultAutoData]
    public void ExtendedListTest(
        ExtendedList<int> list, 
        IExtendedList<int> listInterf)
    {
        list.Count.Should().NotBe(0);
        list.Should().OnlyHaveUniqueItems();
        listInterf.Count.Should().NotBe(0);
        listInterf.Should().OnlyHaveUniqueItems();
    }
    
    [Theory, DefaultAutoData]
    public void ExtendedListClassTest(
        ExtendedList<TestClass> list, 
        IExtendedList<TestClass> listInterf)
    {
        list.Count.Should().NotBe(0);
        list.Should().OnlyHaveUniqueItems();
        listInterf.Count.Should().NotBe(0);
        listInterf.Should().OnlyHaveUniqueItems();
    }

    public record TestClass(int Int, string String);
}