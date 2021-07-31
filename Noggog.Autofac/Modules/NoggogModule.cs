using Autofac;
using Noggog.IO;
using Noggog.Utility;

namespace Noggog.Autofac.Modules
{
    public class NoggogModule : global::Autofac.Module
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
                .InNamespaceOf<IDeleteEntireDirectory>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}