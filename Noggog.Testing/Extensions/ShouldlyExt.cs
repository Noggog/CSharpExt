using System.Reflection;
using System.Runtime.CompilerServices;
using Shouldly;

namespace Noggog.Testing.Extensions;

public static class ShouldlyExt
{
    private static bool RoughlyEqual<TLhs>(TLhs actual, object? expected)
    {
        if (actual == null && expected == null) return true;
        if (actual == null || expected == null) return false;
        if (object.Equals(actual, expected)) return true;
        
        try
        {
            TLhs? convertedExpected = (TLhs?)Convert.ChangeType(expected, actual.GetType());
            if (object.Equals(actual, convertedExpected)) return true;
        }
        catch (Exception e)
        {
        }
        
        if (expected is IConvertible convertibleExpected)
        {
            try
            {
                var convertedExpected = convertibleExpected.ToType(actual.GetType(), null);
                if (object.Equals(actual, convertedExpected)) return true;
            }
            catch (Exception e)
            {
            }
        }

        return true;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqual<T>(this IEnumerable<T> actual, params object?[] expected)
    {
        ShouldEqual<T>(actual, (IEnumerable<object?>)expected);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqual<T>(this IEnumerable<T> actual, IEnumerable<object?> expected)
    {
        var actualList = actual.ToArray();
        var expectedList = expected.ToArray();
        if (actualList.Length != expectedList.Length)
        {
            throw new ShouldAssertException(
                new ExpectedActualShouldlyMessage(expectedList, actualList, null).ToString());
        }

        if (actualList.Length == 0) return;

        for (int i = 0; i < actualList.Length; i++)
        {
            if (!RoughlyEqual(actualList[i], expectedList[i]))
            {
                throw new ShouldAssertException(
                    new ExpectedActualShouldlyMessage(expected, actual, null).ToString()); 
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqual<TLhs, TRhs>(this TLhs actual, TRhs expected)
    {
        if (actual is IEnumerable<object> actualEnumerable && expected is IEnumerable<object> expectedEnumerable)
        {
            ShouldEqual(actualEnumerable, expectedEnumerable);
            return;
        }
        if (RoughlyEqual(actual, expected)) return;

        throw new ShouldAssertException(
            new ExpectedActualShouldlyMessage(expected, actual, null).ToString());
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
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldAllBe<T>(this IEnumerable<T> actual, T item)
    {
        actual.ShouldAllBe<T>(x => Equals(x, item));    
    }
}