using System.Reflection;
using System.Runtime.CompilerServices;
using Shouldly;

namespace Noggog.Testing.Extensions;

public static class ShouldlyExt
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldBe<T>(this IEnumerable<T> actual, params T[] vals)
    {
        ShouldBeTestExtensions.ShouldBe(actual, vals);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IEnumerable<MethodInfo> Methods(this Type type)
    {
        return type.GetMethods()
            .Where(m => m.Name is not "GetType" and not "ToString" and not "GetHashCode" and not "Equals");
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<T>(this IEnumerable<T> actual, int count)
    {
        actual.Count().ShouldBe(count);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<T>(this IReadOnlyCollection<T> actual, int count)
    {
        actual.Count.ShouldBe(count);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldHaveCount<T>(this T[] actual, int count)
    {
        actual.Length.ShouldBe(count);
    }
}