using Noggog;
using Noggog.Autofac.Validation;
using Noggog.Autofac.Validation.Rules;
using Noggog.Testing.AutoFixture;
using NSubstitute;
using Shouldly;

namespace CSharpExt.UnitTests.Autofac;

public class CheckIsDelegateFactoryTests
{
    private interface IClassWithFactory
    {
    }

    class ClassWithFactory : IClassWithFactory
    {
        public delegate ClassWithFactory Factory(string str, int i);
        public delegate IClassWithFactory InterfaceFactory(string str, int i);
            
        public ClassWithFactory(string str, int i, string otherParam)
        {
        }
    }
        
    [Theory, TestData]
    public void Typical(CheckIsDelegateFactory sut)
    {
        sut.IsAllowed(typeof(ClassWithFactory.Factory))
            .ShouldBeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(ClassWithFactory),
            Arg.Is<HashSet<string>>(x => x.SetEquals("str", "i")));
        sut.ValidateType.Received(1).Validate(typeof(ClassWithFactory), false);
    }
        
    [Theory, TestData]
    public void InterfaceReturn(CheckIsDelegateFactory sut)
    {
        sut.Registrations.Items.TryGetValue(typeof(IClassWithFactory), out _).Returns(x =>
        {
            x[1] = new List<Registration>() { new(typeof(ClassWithFactory), true)};
            return true;
        });
        sut.IsAllowed(typeof(ClassWithFactory.InterfaceFactory))
            .ShouldBeTrue();
        sut.ValidateTypeCtor.Received(1).Validate(typeof(ClassWithFactory),
            Arg.Is<HashSet<string>>(x => x.SetEquals("str", "i")));
        sut.ValidateType.Received(1).Validate(typeof(IClassWithFactory), false);
    }
        
    [Theory, TestData]
    public void InterfaceReturnNoValidationNeeded(CheckIsDelegateFactory sut)
    {
        sut.Registrations.Items.TryGetValue(typeof(IClassWithFactory), out _).Returns(x =>
        {
            x[1] = new List<Registration>() { new(typeof(ClassWithFactory), false)};
            return true;
        });
        sut.IsAllowed(typeof(ClassWithFactory.InterfaceFactory))
            .ShouldBeTrue();
        sut.ValidateTypeCtor.DidNotReceiveWithAnyArgs().Validate(default!);
        sut.ValidateType.Received(1).Validate(typeof(IClassWithFactory), false);
    }
        
    [Theory, TestData]
    public void RandomType(CheckIsDelegateFactory sut)
    {
        sut.IsAllowed(typeof(string))
            .ShouldBeFalse();
    }
}