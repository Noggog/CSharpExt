using Autofac;
using Noggog.Autofac.Validation;
using Xunit;

namespace CSharpExt.UnitTests.Autofac.Integration
{
    public class DelegateFactoryReturn : IClassFixture<ValidationFixture>
    {
        private readonly ValidationFixture _validationFixture;

        public DelegateFactoryReturn(ValidationFixture validationFixture)
        {
            _validationFixture = validationFixture;
        }

        public class OtherClass
        {
            public delegate OtherClass Factory(int i);

            public OtherClass(int i)
            {
                
            }
        }

        record TopLevel(OtherClass.Factory service);

        [Fact]
        public void Test()
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
    }
}