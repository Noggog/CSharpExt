using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Shouldly;

namespace Noggog.Testing.Extensions;

public static class ShouldlyExt
{
    public static object? ConvertWithImplicitOperator(object input, Type targetType)
    {
        var inputType = input.GetType();

        // Look for implicit operator in input type
        var method = inputType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "op_Implicit" &&
                m.ReturnType == targetType &&
                m.GetParameters()[0].ParameterType.IsAssignableFrom(inputType));

        // Or look for it in target type
        method ??= targetType
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "op_Implicit" &&
                m.ReturnType == targetType &&
                m.GetParameters()[0].ParameterType.IsAssignableFrom(inputType));

        if (method == null)
            throw new InvalidCastException($"No implicit conversion from {inputType} to {targetType}");

        return method.Invoke(null, new[] { input });
    }
    
    internal static bool RoughlyEqual<TLhs>(TLhs actual, object? expected)
    {
        if (actual == null && expected == null) return true;
        if (actual == null || expected == null) return false;
        if (object.Equals(actual, expected)) return true;

        try
        {
            TLhs? convertedExpected = (TLhs?)ConvertWithImplicitOperator(expected, actual.GetType());
            if (object.Equals(actual, convertedExpected)) return true;
        }
        catch (Exception e)
        {
        }
        
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

        return false;
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqualEnumerable<T, TRhs>(this IEnumerable<T>? actual, params TRhs?[] expected)
        where TRhs : T 
    {
        ShouldEqualEnumerableInternal(actual, expected);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqualEnumerable<T>(this IEnumerable<T>? actual, params object?[] expected)
    {
        ShouldEqualEnumerableInternal(actual, expected);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqualEnumerable<TLhs, TRhs>(this IEnumerable<TLhs>? actual, IEnumerable<TRhs>? expected)
    {
        ShouldEqualEnumerableInternal(actual, expected);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ShouldEqualEnumerable<TLhs, TRhs>(this IEnumerable<TLhs>? actual, ReadOnlyMemorySlice<TRhs>? expected)
    {
        ShouldEqualEnumerableInternal(actual, expected);
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ShouldEqualEnumerableInternal(IEnumerable? actual, IEnumerable? expected)
    {
        if (actual == null && expected == null) return;
        if (actual == null || expected == null)
        {
            throw new ShouldAssertException(
                new ExpectedActualShouldlyMessage(expected, actual, null).ToString());
        }
        var actualList = new List<object>();
        foreach (var item in actual)
        {
            actualList.Add(item);
        }
        var expectedList = new List<object>();
        foreach (var item in expected)
        {
            expectedList.Add(item);
        }
        if (actualList.Count != expectedList.Count)
        {
            throw new ShouldAssertException(
                new ExpectedActualShouldlyMessage(expectedList, actualList, null).ToString());
        }

        if (actualList.Count == 0) return;

        for (int i = 0; i < actualList.Count; i++)
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