using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class GetResponseBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type t
            && IsGetResponseType(t))
        {
            return GetCreateMethod()
                .MakeGenericMethod(t.GenericTypeArguments[0])
                .Invoke(this, new object[] {context, true})!;
        }
        return new NoSpecimen();
    }

    public static bool IsGetResponseType(Type t)
    {
        return t.GenericTypeArguments.Length == 1
               && t.Name.StartsWith("GetResponse");
    }

    public static MethodInfo GetCreateMethod()
    {
        return typeof(GetResponseBuilder)
            .GetMethod("CreateGetResponse", BindingFlags.Static | BindingFlags.Public)!;
    }

    public static GetResponse<T> CreateGetResponse<T>(
        ISpecimenContext context,
        bool successful)
    {
        return GetResponse<T>.Create(
            successful: successful,
            successful ? context.Create<T>() : default(T),
            context.Create<string>());
    }
}