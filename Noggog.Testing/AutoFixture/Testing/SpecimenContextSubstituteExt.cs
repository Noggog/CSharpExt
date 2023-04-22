using AutoFixture.Kernel;
using NSubstitute;
using NSubstitute.Core;

namespace Noggog.Testing.AutoFixture.Testing;

public static class SpecimenContextSubstituteExt
{
    public static void ShouldHaveCreated<T>(this ISpecimenContext context)
    {
        context.Received(1).Resolve(Arg.Is<SeededRequest>(o => (Type) o.Request == typeof(T)));
    }
        
    public static ConfiguredCall MockToReturn<T>(this ISpecimenContext context, T item)
    {
        return context.Resolve(Arg.Is<SeededRequest>(o => (Type)o.Request == typeof(T)))
            .Returns(item!);
    }
        
    public static ConfiguredCall MockToReturn<T>(this ISpecimenContext context)
        where T : class
    {
        return context.Resolve(Arg.Is<SeededRequest>(o => (Type)o.Request == typeof(T)))
            .Returns(Substitute.For<T>());
    }
        
    public static void FillAllProperties<T>(this ISpecimenContext context, T item)
        where T : class
    {
        foreach (var prop in item.GetType().GetProperties())
        {
            var setter = prop.GetSetMethod();
            if (setter == null) continue;
            setter.Invoke(item, new object[] { context.Resolve(prop.PropertyType) });
        }
    }
}