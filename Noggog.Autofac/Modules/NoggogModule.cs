using Autofac;
using Noggog.IO;
using Noggog.Processes;
using Noggog.Reactive;
using Noggog.Time;
using Noggog.Tooling.WorkEngine;

namespace Noggog.Autofac.Modules;

public class NoggogModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IDeepCopyDirectory).Assembly)
            .InNamespacesOf(
                typeof(IDeleteEntireDirectory),
                typeof(INowProvider),
                typeof(IProcessFactory),
                typeof(IWatchFile))
            .Except<TempFile>()
            .Except<TempFolder>()
            .Except<ProcessWrapper>()
            .NotInjection()
            .AsImplementedInterfaces()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(IWorkDropoff).Assembly)
            .InNamespacesOf(
                typeof(IWorkDropoff))
            .NotInjection()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}