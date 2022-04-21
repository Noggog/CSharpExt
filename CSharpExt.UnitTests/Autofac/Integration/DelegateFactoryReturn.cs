using Autofac;
using Noggog.Autofac.Validation;
using Xunit;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class DelegateFactoryReturn : IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public DelegateFactoryReturn(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }
        
    public interface ISubClass
    {
    }
        
    public class SubClass : ISubClass
    {
    }

    public interface IFactoryClass
    {
    }

    public class FactoryClass : IFactoryClass
    {
        public delegate FactoryClass Factory(int i);
        public delegate IFactoryClass InterfaceFactory(int i);

        public FactoryClass(int i, ISubClass subClass)
        {
        }
    }

    record TopLevel(FactoryClass.Factory service);

    record InterfaceFactoryTopLevel(FactoryClass.InterfaceFactory service);

    [Fact]
    public void FactoryTargetNotRegistered()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TopLevel>().AsSelf();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(TopLevel));
        });
    }
        
    [Fact]
    public void FactoryReturnsInstance()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<InterfaceFactoryTopLevel>().AsSelf();
        builder.RegisterType<FactoryClass>().As<IFactoryClass>();
        builder.RegisterType<SubClass>().As<ISubClass>();
        var cont = builder.Build();
        using var disp = _validationFixture.GetValidator(cont, out var validate);
        validate.Validate(typeof(InterfaceFactoryTopLevel));
    }
        
    [Fact]
    public void FactoryReturnsInstanceAndValidates()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<InterfaceFactoryTopLevel>().AsSelf();
        builder.RegisterType<FactoryClass>().As<IFactoryClass>();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(InterfaceFactoryTopLevel));
        });
    }
}