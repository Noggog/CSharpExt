using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;

namespace Noggog.Testing.AutoFixture;

public class Array2dBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is Type type
            && IsTargetType(type))
        {
            return GetCreateMethod()
                .MakeGenericMethod(type.GenericTypeArguments[0])
                .Invoke(this, new object[] {context, request})!;
        }
        
        return new NoSpecimen();
    }

    public static bool IsTargetType(Type t)
    {
        return t.GenericTypeArguments.Length == 1
               && t.Name.Contains("Array2d");
    }

    public static MethodInfo GetCreateMethod()
    {
        return typeof(Array2dBuilder)
            .GetMethod("Create", BindingFlags.Static | BindingFlags.Public)!;
    }

    public static IArray2d<T> Create<T>(
        ISpecimenContext context,
        object request)
    {
        var arr = new Array2d<T>(3, 3);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                arr[i, j] = context.Create<T>();
            }
        }

        return arr;
    }
}