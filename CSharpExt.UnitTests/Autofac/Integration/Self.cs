using Autofac;
using Noggog.Autofac;
using Noggog.Autofac.Validation;
using Xunit;

namespace CSharpExt.UnitTests.Autofac.Integration
{
    public class Self
    {
        [Fact]
        public void ValidateSelf()
        {
            using var scope = ValidationMixIn.Container.BeginLifetimeScope(cfg =>
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<ValidationModule>();
                builder.RegisterInstance(ValidationMixIn.Container).As<IContainer>();
                cfg.RegisterInstance(builder.Build()).As<IContainer>();
            });
            scope.Resolve<IValidator>().Validate(typeof(IValidator));
        } 
    }
}