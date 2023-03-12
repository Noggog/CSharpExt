using Autofac;
using Noggog.Autofac.Validation;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class TopLevelValidation : IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public TopLevelValidation(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }

    interface IOtherClass {}
    record OtherClass() : IOtherClass;

    interface ITopLevel {}
    record TopLevel(IOtherClass OtherClass) : ITopLevel;

    [Fact]
    public void Interface()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<TopLevel>().As<ITopLevel>();
        var cont = builder.Build();
        Assert.Throws<AutofacValidationException>(() =>
        {
            using var disp = _validationFixture.GetValidator(cont, out var validate);
            validate.Validate(typeof(ITopLevel));
        });
    }
}