using System.Reflection;
using Xunit;
using Noggog;

namespace CSharpExt.UnitTests;

public class TypeExt_Tests
{
    #region InheritsFrom
    class AClass
    {
    }

    class SubClass : AClass, IInterface
    {
    }

    class RandomClass
    {
    }

    interface IInterface
    {
    }

    class GenClass<T>
        where T : AClass
    {
    }

    interface IGenInterface<T>
        where T : AClass
    {
    }

    class SubGenClass : GenClass<AClass>, IGenInterface<AClass>
    {
    }

    class SubSubGenClass : GenClass<SubClass>, IGenInterface<SubClass>
    {
    }

    class SubGenGenClass<T> : GenClass<T>, IGenInterface<T>
        where T : SubClass
    {
    }

    [Fact]
    public void Typical()
    {
        Assert.True(typeof(SubClass).InheritsFrom(typeof(AClass)));
    }

    [Fact]
    public void Typical_Fail()
    {
        Assert.False(typeof(RandomClass).InheritsFrom(typeof(AClass)));
    }

    [Fact]
    public void Typical_Inverted_Fail()
    {
        Assert.False(typeof(AClass).InheritsFrom(typeof(SubClass)));
    }

    [Fact]
    public void Interface()
    {
        Assert.True(typeof(SubClass).InheritsFrom(typeof(IInterface)));
    }

    [Fact]
    public void Interface_Fail()
    {
        Assert.False(typeof(RandomClass).InheritsFrom(typeof(IInterface)));
    }

    [Fact]
    public void Interface_Inverted_Fail()
    {
        Assert.False(typeof(IInterface).InheritsFrom(typeof(SubClass)));
    }

    [Fact]
    public void Typical_Self()
    {
        Assert.True(typeof(AClass).InheritsFrom(typeof(AClass), excludeSelf: false));
    }

    [Fact]
    public void Typical_Self_Fail()
    {
        Assert.False(typeof(AClass).InheritsFrom(typeof(AClass), excludeSelf: true));
    }

    [Fact]
    public void Interface_Self()
    {
        Assert.True(typeof(IInterface).InheritsFrom(typeof(IInterface), excludeSelf: false));
    }

    [Fact]
    public void Interface_Self_Fail()
    {
        Assert.False(typeof(IInterface).InheritsFrom(typeof(IInterface), excludeSelf: true));
    }

    [Fact]
    public void Typical_Self_Subclass()
    {
        Assert.True(typeof(SubClass).InheritsFrom(typeof(SubClass), excludeSelf: false));
    }

    [Fact]
    public void Typical_Self_Subclass_Fail()
    {
        Assert.False(typeof(SubClass).InheritsFrom(typeof(SubClass), excludeSelf: true));
    }

    [Fact]
    public void GenInterface_Sub()
    {
        Assert.True(typeof(IGenInterface<SubClass>).InheritsFrom(typeof(IGenInterface<AClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_Sub_Fail()
    {
        Assert.False(typeof(IGenInterface<AClass>).InheritsFrom(typeof(IGenInterface<SubClass>), excludeSelf: true));
    }

    [Fact]
    public void GenInterface_Self()
    {
        Assert.True(typeof(IGenInterface<AClass>).InheritsFrom(typeof(IGenInterface<AClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_Self_Fail()
    {
        Assert.False(typeof(IGenInterface<AClass>).InheritsFrom(typeof(IGenInterface<AClass>), excludeSelf: true));
    }

    [Fact]
    public void GenInterface_Sub_Self()
    {
        Assert.True(typeof(IGenInterface<SubClass>).InheritsFrom(typeof(IGenInterface<SubClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_Sub_Self_Fail()
    {
        Assert.False(typeof(IGenInterface<SubClass>).InheritsFrom(typeof(IGenInterface<SubClass>), excludeSelf: true));
    }

    [Fact]
    public void GenInterface_SubClass()
    {
        Assert.True(typeof(SubGenClass).InheritsFrom(typeof(IGenInterface<AClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_SubClass_InterfaceSubclassed()
    {
        Assert.False(typeof(SubGenClass).InheritsFrom(typeof(IGenInterface<SubClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_Unspecified()
    {
        //ToDo
        //Assert.True(typeof(SubGenGenClass<>).InheritsFrom(typeof(IGenInterface<>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_SubUnspecified_Fail()
    {
        Assert.True(typeof(SubGenGenClass<>).InheritsFrom(typeof(IGenInterface<AClass>), excludeSelf: false));
    }

    [Fact]
    public void GenInterface_BaseUnspecified_Fail()
    {
        Assert.False(typeof(SubGenGenClass<SubClass>).InheritsFrom(typeof(IGenInterface<>), excludeSelf: false));
    }

    [Fact]
    public void Generic_Specified()
    {
        Assert.True(typeof(SubGenClass).InheritsFrom(typeof(GenClass<AClass>)));
    }

    [Fact]
    public void Generic_Self_GenSpecified()
    {
        Assert.True(typeof(GenClass<AClass>).InheritsFrom(typeof(GenClass<AClass>)));
    }

    [Fact]
    public void Generic_GenSpecified()
    {
        Assert.True(typeof(GenClass<SubClass>).InheritsFrom(typeof(GenClass<AClass>)));
    }

    [Fact]
    public void Generic_Specified_Inverted_Fail()
    {
        Assert.False(typeof(GenClass<AClass>).InheritsFrom(typeof(SubGenClass)));
    }

    [Fact]
    public void Generic_Specified_Subclass()
    {
        Assert.True(typeof(SubSubGenClass).InheritsFrom(typeof(GenClass<AClass>)));
    }

    [Fact]
    public void Generic_Specified_Subclass_Inverted_Fail()
    {
        Assert.False(typeof(GenClass<AClass>).InheritsFrom(typeof(SubSubGenClass)));
    }

    [Fact]
    public void Generic_Unspecified()
    {
        Assert.True(typeof(SubGenGenClass<SubClass>).InheritsFrom(typeof(GenClass<SubClass>)));
    }

    [Fact]
    public void Generic_Unspecified_Fail()
    {
        Assert.False(typeof(SubGenGenClass<>).InheritsFrom(typeof(GenClass<SubClass>)));
    }

    [Fact]
    public void Generic_SubUnspecified_Fail()
    {
        Assert.False(typeof(SubGenGenClass<>).InheritsFrom(typeof(GenClass<SubClass>)));
    }

    [Fact]
    public void Generic_BaseUnspecified_Inverted_Fail()
    {
        Assert.False(typeof(SubGenGenClass<SubClass>).InheritsFrom(typeof(GenClass<>)));
    }

    [Fact]
    public void Generic_Unspecified_Subclass()
    {
        Assert.True(typeof(SubGenGenClass<SubClass>).InheritsFrom(typeof(GenClass<AClass>)));
    }

    [Fact]
    public void Generic_Unspecified_Subclass_Inverted_Fail()
    {
        Assert.False(typeof(GenClass<AClass>).InheritsFrom(typeof(SubGenGenClass<SubClass>)));
    }

    #region CouldInheritFrom
    [Fact]
    public void Generic_Undefined()
    {
        var methodType = typeof(TypeExt_Tests).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
            .Where((m) => m.Name.Equals(nameof(TestGenericSource))).First();
        Assert.True(typeof(SubClass).InheritsFrom(methodType.ReturnType, couldInherit: true));
    }

    private T? TestGenericSource<T>()
        where T : AClass
    {
        return default(T);
    }
    #endregion
    #endregion
}