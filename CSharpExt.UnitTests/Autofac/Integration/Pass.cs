using Autofac;
using Noggog.Autofac.Validation;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class Pass : IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public Pass(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }

    record COtherClass();
    record CTopLevel(COtherClass OtherClass);
        
    interface IOtherClass {}
    record OtherClass() : IOtherClass;

    interface ITopLevel {}
    record TopLevel(IOtherClass OtherClass) : ITopLevel;

    [Fact]
    public void Concrete()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<CTopLevel>().AsSelf();
        builder.RegisterType<COtherClass>().AsSelf();
        var cont = builder.Build();
        using var disp = _validationFixture.GetValidator(cont, out var validate);
        validate.Validate(typeof(CTopLevel));
    }

    [Fact]
    public void Interface()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TopLevel>().As<ITopLevel>();
        builder.RegisterType<OtherClass>().As<IOtherClass>();
        var cont = builder.Build();
        using var disp = _validationFixture.GetValidator(cont, out var validate);
        validate.Validate(typeof(ITopLevel));
    }
}