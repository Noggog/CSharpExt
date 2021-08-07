using Autofac;
using Autofac.Builder;
using NSubstitute;

namespace Noggog
{
    public static class RegistrationBuilderExt
    {
        public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterMock<T>(this ContainerBuilder builder)
            where T : class
        {
            return builder.RegisterInstance(Substitute.For<T>()).As<T>();
        }
    }
}