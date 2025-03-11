using Noggog;
using Noggog.Testing.AutoFixture;
using Shouldly;

namespace CSharpExt.UnitTests.AutoFixture;

public class ExtendedListBuilderTests
{
    [Theory, DefaultAutoData]
    public void ExtendedListTest(
        ExtendedList<int> list, 
        IExtendedList<int> listInterf)
    {
        list.Count.ShouldNotBe(0);
        list.ShouldBeUnique();
        listInterf.Count.ShouldNotBe(0);
        listInterf.ShouldBeUnique();
    }
    
    [Theory, DefaultAutoData]
    public void ExtendedListClassTest(
        ExtendedList<TestClass> list, 
        IExtendedList<TestClass> listInterf)
    {
        list.Count.ShouldNotBe(0);
        list.ShouldBeUnique();
        listInterf.Count.ShouldNotBe(0);
        listInterf.ShouldBeUnique();
    }

    public record TestClass(int Int, string String);
}