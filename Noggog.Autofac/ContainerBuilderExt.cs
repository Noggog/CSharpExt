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
}
