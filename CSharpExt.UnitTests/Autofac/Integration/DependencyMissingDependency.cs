using Autofac;
using Noggog.Autofac.Validation;
using Xunit;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class DependencyMissingDependency : IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public DependencyMissingDependency(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }

    record CMissing();
    record COtherClass(CMissing m);
    record CTopLevel(COtherClass OtherClass);

    interface IMissing {}
    record Missing() : IMissing;
        
    interface IOtherClass {}
    record OtherClass(IMissing m) : IOtherClass;

    interface ITopLevel {}
    record TopLevel(IOtherClass OtherClass) : ITopLevel;

    [Fact]
    public void Concrete()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<CTopLevel>().AsSelf();
        builder.RegisterType<COtherClass>().AsSelf();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(CTopLevel));
        });
    }

    [Fact]
    public void Interface()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TopLevel>().As<ITopLevel>();
        builder.RegisterType<OtherClass>().As<IOtherClass>();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(ITopLevel));
        });
    }
}