using Autofac;

namespace Noggog.Autofac.Validation;

public class ValidationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ValidateTypes>().AsSelf();
        builder.RegisterAssemblyTypes(typeof(IValidator).Assembly)
            .InNamespaceOf<IValidator>()
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
    }
}