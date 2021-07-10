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
        
        public static void ValidateRegistrations(this IContainer container, params Type[] extraUsages)
        {
            ValidateRegistrations(container, true, extraUsages);
        }

        public static void ValidateRegistrations(this IContainer container, bool evaluateUsages, params Type[] extraUsages)
        {
            using var scope = Container.BeginLifetimeScope(cfg =>
            {
                cfg.RegisterInstance(container).As<IContainer>();
            });
            scope.Resolve<IValidate>()
                .ValidateRegistrations(evaluateUsages: evaluateUsages, extraUsages);
        }
    }
}