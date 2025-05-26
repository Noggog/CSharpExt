using Noggog.Autofac.Validation;
using Noggog.Testing.Extensions;
using Shouldly;

namespace CSharpExt.UnitTests.Autofac;

public class FillUsagesTests
{
    class NoCtorClass
    {
    }

    class EmptyCtorClass
    {
        public EmptyCtorClass()
        {
                
        }
    }

    [Fact]
    public void EmptyCtors()
    {
        new GetUsages().Get(
                typeof(NoCtorClass),
                typeof(EmptyCtorClass))
            .ShouldBeEmpty();
    }

    class SomeParams
    {
        public SomeParams(
            NoCtorClass otherClass,
            SubClass subClass)
        {
        }
    }

    class SubClass
    {
        public SubClass(EmptyCtorClass otherClass)
        {
                
        }
    }

    [Fact]
    public void Typical()
    {
        new GetUsages().Get(
                typeof(SomeParams))
            .ShouldEqualEnumerable(
                typeof(NoCtorClass),
                typeof(SubClass));
    }
}