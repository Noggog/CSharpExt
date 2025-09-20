using Autofac;
using Noggog.DotNetCli.DI;
using Noggog.IO;
using Noggog.Processes.DI;
using Noggog.Reactive;
using Noggog.Time;
using Noggog.WorkEngine;

namespace Noggog.Autofac.Modules;

public class NoggogModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IDeepCopyDirectory).Assembly)
            .InNamespacesOf(
                typeof(IDeleteEntireDirectory),
                typeof(INowProvider),
                typeof(IProcessRunner),
                typeof(IQueryNugetListing),
                typeof(IWatchFile))
            .Except<TempFile>()
            .Except<TempFolder>()
            .NotInjection()
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}