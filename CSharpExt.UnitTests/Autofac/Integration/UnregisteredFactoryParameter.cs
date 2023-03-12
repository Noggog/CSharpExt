using Autofac;
using Noggog.Autofac.Validation;

namespace CSharpExt.UnitTests.Autofac.Integration;

public class UnregisteredFactoryParameter: IClassFixture<ValidationFixture>
{
    private readonly ValidationFixture _validationFixture;

    public UnregisteredFactoryParameter(ValidationFixture validationFixture)
    {
        _validationFixture = validationFixture;
    }

    record Missing();

    class OtherClass
    {
        public delegate OtherClass Factory(Missing m);

        public OtherClass(Missing m)
        {
        }
    }

    record TopLevel(OtherClass.Factory OtherClass);

    [Fact]
    public void Test()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<OtherClass>().AsSelf();
        builder.RegisterType<TopLevel>().AsSelf();
        var cont = builder.Build();
        using var disp = _validationFixture.GetValidator(cont, out var validate);
        validate.Validate(typeof(TopLevel));
    }
}