using Autofac;
using Noggog.Autofac.Validation;
using Xunit;

namespace CSharpExt.UnitTests.Autofac.Integration
{
    public class ConcreteInsteadOfInterface : IClassFixture<ValidationFixture>
    {
        private readonly ValidationFixture _validationFixture;

        public ConcreteInsteadOfInterface(ValidationFixture validationFixture)
        {
            _validationFixture = validationFixture;
        }
        
        interface IService {}
        record Service() : IService;

        record TopLevel(Service service);

        [Fact]
        public void Test()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<TopLevel>().AsSelf();
            builder.RegisterType<Service>().As<IService>();
            var cont = builder.Build();
            Assert.Throws<AutofacValidationException>(() =>
            {
                using var disp = _validationFixture.GetValidator(cont, out var validate);
                validate.Validate(typeof(TopLevel));
            });
        }
    }
}