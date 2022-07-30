using Autofac;
using Noggog.IO;
using Noggog.Reactive;
using Noggog.Time;
using Noggog.Tooling.WorkEngine;
using Noggog.Utility;

namespace Noggog.Autofac.Modules;

public class NoggogModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TempFileProvider>().As<ITempFileProvider>()
            .SingleInstance();
        builder.RegisterType<TempFolderProvider>().As<ITempFolderProvider>()
            .SingleInstance();
        builder.RegisterType<ProcessFactory>().As<IProcessFactory>()
            .SingleInstance();
        builder.RegisterAssemblyTypes(typeof(IDeepCopyDirectory).Assembly)
            .InNamespacesOf(
                typeof(IDeleteEntireDirectory),
                typeof(INowProvider),
                typeof(IWatchFile))
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