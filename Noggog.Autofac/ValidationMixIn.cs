using System;
using Autofac;
using Noggog.Autofac.Validation;

namespace Noggog.Autofac
{
    public static class ValidationMixIn
    {
        internal static readonly IContainer Container;
        
        static ValidationMixIn()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<ValidationModule>();
            Container = builder.Build();
        }
        
        public static void ValidateEverything(this IContainer container)
        {
            using var scope = Container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterInstance(container).As<IContainer>();
            });
            scope.Resolve<IValidator>()
                .ValidateEverything();
        }

        public static void Validate(this IContainer container, Type drivingType, params Type[] otherDrivingTypes)
        {
            using var scope = Container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterInstance(container).As<IContainer>();
            });
            scope.Resolve<IValidator>()
                .Validate(drivingType, otherDrivingTypes);
        }
    }
}