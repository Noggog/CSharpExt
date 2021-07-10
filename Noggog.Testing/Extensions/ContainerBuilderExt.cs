using Autofac;
using FakeItEasy;

namespace Noggog
{
    public static class RegistrationBuilderExt
    {
        public static void RegisterMock<T>(this ContainerBuilder builder, bool strict = false)
            where T : class
        {
            if (strict)
            {
                builder.RegisterInstance(A.Fake<T>(x => x.Strict())).As<T>();
            }
            else
            {
                builder.RegisterInstance(A.Fake<T>()).As<T>();
            }
        }
    }
}