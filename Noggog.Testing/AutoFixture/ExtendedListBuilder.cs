using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class ExtendedListBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is SeededRequest seed)
        {
            request = seed.Request;
        }

        if (request is not Type t
            || !IsTargetType(t)) return new NoSpecimen();
        
        return GetCreateMethod()
            .MakeGenericMethod(t.GenericTypeArguments[0])
            .Invoke(this, new object[] {context, request})!;
    }

    public static bool IsTargetType(Type t)
    {
        if (t.GenericTypeArguments.Length != 1) return false;
        return t.Name switch
        {
            "ExtendedList`1" => true,
            "IExtendedList`1" => true,
            _ => false
        };
    }

    public static MethodInfo GetCreateMethod()
    {
        return typeof(ExtendedListBuilder)
            .GetMethod("CreateList", BindingFlags.Static | BindingFlags.Public)!;
    }

    public static ExtendedList<T> CreateList<T>(
        ISpecimenContext context,
        object request)
    {
        return new ExtendedList<T>(context.Create<T[]>());
    }
}