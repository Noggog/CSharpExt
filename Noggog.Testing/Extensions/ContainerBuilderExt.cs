using Autofac;
using NSubstitute;

namespace Noggog
{
    public static class RegistrationBuilderExt
    {
        public static void RegisterMock<T>(this ContainerBuilder builder)
            where T : class
        {
            builder.RegisterInstance(Substitute.For<T>()).As<T>();
        }
    }
}