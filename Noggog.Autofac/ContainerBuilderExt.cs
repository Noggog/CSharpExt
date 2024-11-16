using Autofac;
using System.IO.Abstractions;

namespace Noggog.Autofac;

public static class ContainerBuilderExt
{
    public static void WithTypicalFilesystem(this ContainerBuilder builder)
    {
        builder.RegisterType<FileSystem>().As<IFileSystem>()
            .SingleInstance();
    }

    public static void TypicalSingletonFolderRegistration<TPrototype>(this ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(TPrototype).Assembly)
            .InNamespacesOf(
                typeof(TPrototype))
            .AsImplementedInterfaces()
            .AsSelf()
            .SingleInstance();
    }

    public static void TypicalTransientFolderRegistration<TPrototype>(this ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(TPrototype).Assembly)
            .InNamespacesOf(
                typeof(TPrototype))
            .AsImplementedInterfaces()
            .AsSelf();
    }
}
