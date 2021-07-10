using Autofac;

namespace Noggog.Autofac.Validation
{
    public class ValidationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ValidateAllRegistrations>().AsSelf();
            builder.RegisterAssemblyTypes(typeof(IValidate).Assembly)
                .InNamespaceOf<IValidate>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
        }
    }
}