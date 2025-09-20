using Autofac;
using Noggog.WorkEngine;

namespace Noggog.Autofac.Modules;

public class WorkEngineModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IWorkDropoff).Assembly)
            .InNamespacesOf(
                typeof(IWorkDropoff))
            .Except<NumWorkThreadsConstant>()
            .NotInjection()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}