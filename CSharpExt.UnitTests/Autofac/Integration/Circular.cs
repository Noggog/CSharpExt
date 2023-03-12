using Autofac;
using Noggog.Autofac.Validation;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class Circular : IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public Circular(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }
        
    record ClassA(ClassB b);
    record ClassB(ClassA a);
        
    [Fact]
    public void DirectCircular()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<ClassA>().AsSelf();
        builder.RegisterType<ClassB>().AsSelf();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(ClassA), typeof(ClassB));
        });
    }
        
    interface IInterface {}
    record ClassC(ClassD d) : IInterface;
    record ClassD(IInterface c);
        
    [Fact]
    public void InterfaceMiddleman()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<ClassC>().As<IInterface>();
        builder.RegisterType<ClassD>().AsSelf();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(ClassD), typeof(IInterface));
        });
    }
}