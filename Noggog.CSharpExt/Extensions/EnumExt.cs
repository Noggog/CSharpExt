using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace Noggog;

public static class EnumExt
{
    [Pure]
    [Obsolete("Use NumEntries instead")]
    public static int Length(this Enum obj) 
    { 
        return Enum.GetValues(obj.GetType()).Length; 
    } 
 
    [Pure]
    [Obsolete("Use Enums<T>.NumEntries instead")]
    public static int Length<T>() 
        where T : struct, Enum
    {
        return Enums<T>.NumEntries;
    }
 
    [Pure]
    [Obsolete("Use Enums<T>.TryConvert instead")]
    public static bool TryParse<T>(int number, out T val) 
        where T : struct, Enum
    {
        return Enums<T>.TryConvert(number, out val);
    }
 
    [Pure]
    [Obsolete("Use Enums<T>.TryConvert instead")]
    public static T Parse<T>(int number, T defaultTo) 
        where T : struct, Enum 
    { 
        return Enums<T>.TryConvert(number, defaultTo);
    } 
 
    [Pure]
    [Obsolete("Use Enums<T>.Values instead")]
    public static IEnumerable<T> GetValues<T>() 
        where T : struct, Enum 
    { 
        return Enums<T>.Values; 
    } 
 
    [Obsolete("Use Enums<T>.NumEntries instead")]
    public static int GetSize<T>() 
        where T : struct, Enum 
    { 
        return Enums<T>.NumEntries;
    } 
 
    [Obsolete("Use Enums.GetNth instead")]
    public static bool GetNth(Type t, int n, [MaybeNullWhen(false)] out Enum e)
    {
        return Enums.TryGetNth(t, n, out e);
    } 
 
    [Obsolete("Use Enums<T>.GetNth instead")]
    public static T GetNth<T>(int n) 
        where T : struct, Enum 
    { 
        return Enums<T>.GetNth(n);
    } 
 
    [Obsolete("Use Enums<T>.GetNth instead")]
    public static T GetNth<T>(int n, T defaultPick) 
        where T : struct, Enum 
    { 
        return Enums<T>.GetNth(n, defaultPick);
    }
    
    [Obsolete("Use ToStringFast instead")]
    public static string ToStringFast_Enum_Only<T>(this T e) 
        where T : struct, Enum
    {
        return e.ToStringFast();
    } 
 
    [Obsolete("Use Enums<T>.ToStringFast instead")]
    public static string ToStringFast_Enum_Only<T>(int enumVal) 
        where T : struct, Enum
    {
        return Enums<T>.ToStringFast(enumVal);
    } 
 
    [Obsolete("Use TryToStringFast instead")]
    public static bool TryToStringFast_Enum_Only<T>(this T e, [MaybeNullWhen(false)] out string str) 
        where T : struct, Enum
    {
        return e.TryToStringFast(out str);
    } 
 
    [Obsolete("Use Enums<T>.TryToStringFast instead")]
    public static bool TryToStringFast_Enum_Only<T>(int enumVal, [MaybeNullWhen(false)] out string str) 
        where T : struct, Enum 
    { 
        return Enums<T>.TryToStringFast(enumVal, out str);
    }
    
    [Pure]
    [Obsolete("Use Enums.HasFlag instead")]
    public static bool HasFlag(int value, int flagToCheck)
    {
        return Enums.HasFlag(value, flagToCheck);
    } 
 
    [Pure]
    [Obsolete("Use Enums.HasFlag instead")]
    public static bool HasFlag(uint value, uint flagToCheck) 
    { 
        return Enums.HasFlag(value, flagToCheck);
    } 
 
    [Pure]
    [Obsolete("Use Enums.HasFlag instead")]
    public static bool HasFlag(byte value, byte flagToCheck) 
    { 
        return Enums.HasFlag(value, flagToCheck);
    } 
 
    [Pure]
    [Obsolete("Use Enums.SetFlag instead")]
    public static int SetFlag(int origValue, int flagToSet, bool on) 
    { 
        return Enums.SetFlag(origValue, flagToSet, on);
    } 
 
    [Obsolete("Use Enums.SetFlag instead")]
    public static void SetFlag(ref int origValue, int flagToSet, bool on) 
    { 
        Enums.SetFlag(ref origValue, flagToSet, on);
    } 
 
    [Pure] 
    [Obsolete("Use Enums.SetFlag instead")]
    public static uint SetFlag(uint origValue, uint flagToSet, bool on) 
    { 
        return Enums.SetFlag(origValue, flagToSet, on);
    } 
 
    [Obsolete("Use Enums.SetFlag instead")]
    public static void SetFlag(ref uint origValue, uint flagToSet, bool on) 
    { 
        Enums.SetFlag(ref origValue, flagToSet, on);
    } 
 
    [Pure]
    [Obsolete("Use Enums.SetFlag instead")]
    public static byte SetFlag(byte origValue, byte flagToSet, bool on) 
    { 
        return Enums.SetFlag(origValue, flagToSet, on);
    } 
 
    [Obsolete("Use Enums.SetFlag instead")]
    public static void SetFlag(ref byte origValue, byte flagToSet, bool on) 
    { 
        Enums.SetFlag(ref origValue, flagToSet, on); 
    }

    [Obsolete("Use Enums.TryGetEnumType instead")]
    public static bool TryGetEnumType(string fullTypeName, [MaybeNullWhen(false)] out Type t)
    {
        return Enums.TryGetEnumType(fullTypeName, out t); 
    }
    
    public static int NumEntries<TEnum>(this TEnum obj)
        where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum)).Length;
    }

    public static string ToStringFast<T>(this T e)
        where T : struct, Enum
    {
        return EnumStrings<T>.GetEnumString(e);
    }

    public static bool TryToStringFast<T>(this T e, [MaybeNullWhen(false)] out string str)
        where T : struct, Enum
    {
        return EnumStrings<T>.TryGetEnumString(e, out str);
    }

    public static string ToDescriptionString<TEnum>(this TEnum val)
        where TEnum : struct, Enum
    {
        return Enums<TEnum>.ToDescriptionString(val);
    }
 
    public static string ToDescriptionString(this Enum value) 
    { 
        var fieldInfo = value.GetType().GetField(value.ToString()); 
        if (fieldInfo == null) return string.Empty; 
        var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute))!; 
        return attribute.Description; 
    } 

    public static bool IsFlagsEnum<TEnum>(this TEnum e)
        where TEnum : struct, Enum
    {
        return EnumFlags<TEnum>.IsFlagsEnum;
    }

    public static TEnum SetFlag<TEnum>(this TEnum e, Enum flag, bool on)
        where TEnum : struct, Enum
    {
        var lhs = Convert.ToInt64(e);
        var rhs = Convert.ToInt64(flag);
        if (on)
        {
            lhs |= rhs;
        }
        else
        {
            lhs &= ~rhs;
        }
        return (TEnum)Enum.ToObject(typeof(TEnum), lhs);
    }

    public static TEnum SetFlag<TEnum>(this TEnum? e, Enum flag, bool on)
        where TEnum : struct, Enum
    {
        if (e == null)
        {
            e = new();
        }

        return e.Value.SetFlag(flag, on);
    }

    public static T And<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return Enums<T>.bitwiseOperations.Value.And(value1, value2);
    }
    
    public static T Not<T>(this T value)
        where T : struct, Enum
    {
        return Enums<T>.bitwiseOperations.Value.Not(value);
    }
    
    public static T Or<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return Enums<T>.bitwiseOperations.Value.Or(value1, value2);
    }
    
    public static T Xor<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return Enums<T>.bitwiseOperations.Value.Xor(value1, value2);
    }

    [Pure]
    public static bool HasFlag<T>(this T e, T toCheck)
        where T : struct, Enum
    {
        return Enums<T>.HasFlag(e, toCheck);
    }
    
    public static IEnumerable<TEnum> EnumerateContainedFlags<TEnum>(this TEnum flags, bool includeUndefined = true)
        where TEnum : struct, Enum
    {
        return Enums<TEnum>.EnumerateContainedFlags(flags, includeUndefined: includeUndefined);
    }
}

public static class EnumExt<T> 
    where T : struct, Enum 
{ 
    [Obsolete("Use Enums<T>.Values instead")]
    public static IReadOnlyList<T> Values => Enums<T>.Values;
    
    [Obsolete("Use Enums<T>.Convert instead")]
    public static Func<long, T> Convert { get; } = Enums<T>.Convert;
    
    [Obsolete("Use Enums<T>.ConvertFrom instead")]
    public static Func<T, long> ConvertFrom => Enums<T>.ConvertFrom;
    
    [Obsolete("Use Enums<T>.EnumerateContainedFlags instead")]
    public static IEnumerable<T> EnumerateContainedFlags(T flags, bool includeUndefined = true) 
    { 
        return Enums<T>.EnumerateContainedFlags(flags, includeUndefined);
    }
    
    [Obsolete("Use Enums<T>.IsFlagsEnum instead")]
    public static bool IsFlags => Enums<T>.IsFlagsEnum;
 
    [Obsolete("Use Enums<T>.IsFlagsEnum instead")]
    public static bool IsFlagsEnum()
    {
        return Enums<T>.IsFlagsEnum;
    } 
} 
 