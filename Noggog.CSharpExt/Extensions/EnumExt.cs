using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Noggog;

public static class EnumExt
{
    public static int Length(this Enum obj)
    {
        return Enum.GetValues(obj.GetType()).Length;
    }

    public static int Length<T>()
        where T : struct, Enum
    {
        return Length((Enum)Activator.CreateInstance(typeof(T))!);
    }

    public static bool TryParse<T>(int number, out T val)
        where T : struct, Enum
    {
        if (Enum.IsDefined(typeof(T), number))
        {
            val = (T)(object)number;
            return true;
        }

        val = default;
        return false;
    }

    public static T Parse<T>(int number, T defaultTo)
        where T : struct, Enum
    {
        if (Enum.IsDefined(typeof(T), number))
        {
            return (T)(object)number;
        }
        else
        {
            return defaultTo;
        }
    }

    public static IEnumerable<T> GetValues<T>()
        where T : struct, Enum
    {
        return EnumExt<T>.Values;
    }

    public static int GetSize<T>()
        where T : struct, Enum
    {
        return Enum.GetNames(typeof(T)).Length;
    }

    public static bool GetNth(Type t, int n, out Enum e)
    {
        string[] s = Enum.GetNames(t);
        if (s.Length <= n)
        {
            e = (Enum)Enum.Parse(t, s[0]);
            return false;
        }
        e = (Enum)Enum.Parse(t, s[n]);
        return true;
    }

    public static T GetNth<T>(int n)
        where T : struct, Enum
    {
        return GetNth<T>(n, default(T));
    }

    public static T GetNth<T>(int n, T defaultPick)
        where T : struct, Enum
    {
        Array s = Enum.GetValues(typeof(T));
        if (s.Length <= n) return defaultPick;
        return (T)s.GetValue(n)!;
    }

    public static Dictionary<Type, Dictionary<int, string>> NameDictionary = new Dictionary<Type, Dictionary<int, string>>();

    // Slower
    public static string ToStringFast_Enum_Only<T>(this T e)
        where T : struct, Enum
    {
        IConvertible cv = (IConvertible)e;
        return EnumStrings<T>.GetEnumString(cv.ToInt32(CultureInfo.InvariantCulture));
    }

    // Faster
    public static string ToStringFast_Enum_Only<T>(int enumVal)
        where T : struct, Enum
    {
        return EnumStrings<T>.GetEnumString(enumVal);
    }

    // Slower
    public static bool TryToStringFast_Enum_Only<T>(this T e, [MaybeNullWhen(false)] out string str)
        where T : struct, Enum
    {
        IConvertible cv = (IConvertible)e;
        return EnumStrings<T>.TryGetEnumString(cv.ToInt32(CultureInfo.InvariantCulture), out str);
    }

    // Faster
    public static bool TryToStringFast_Enum_Only<T>(int enumVal, [MaybeNullWhen(false)] out string str)
        where T : struct, Enum
    {
        return EnumStrings<T>.TryGetEnumString(enumVal, out str);
    }

    public static string ToStringFast_Enum_Only(Type enumType, int index)
    {
        if (!NameDictionary.TryGetValue(enumType, out var arr))
        {
            if (enumType.IsEnum)
            {
                arr = new Dictionary<int, string>();
                foreach (Enum value in Enum.GetValues(enumType))
                {
                    arr[((IConvertible)value).ToInt32(CultureInfo.InvariantCulture)] = value.ToString();
                }
                NameDictionary[enumType] = arr;
            }
            else
            {
                throw new Exception("Type must be an enumeration: " + enumType);
            }
        }
        return arr[index];
    }

    public static string ToDescriptionString<TEnum>(this TEnum val)
        where TEnum : struct, Enum
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("T must be an Enum");
        }

        DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }

    public static string ToDescriptionString(this Enum value)
    {
        FieldInfo fieldInfo = value.GetType().GetField(value.ToString())!;
        if (fieldInfo == null) return string.Empty;
        var attribute = (DescriptionAttribute)fieldInfo.GetCustomAttribute(typeof(DescriptionAttribute))!;
        return attribute.Description;
    }

    public static TEnum FromDescriptionString<TEnum>(string value, TEnum defaultValue = default(TEnum))
        where TEnum : struct, Enum
    {
        if (!typeof(TEnum).IsEnum)
        {
            throw new ArgumentException("TEnum must be an Enum");
        }

        if (Enum.TryParse(value, out TEnum parseResult))
        {
            return parseResult;
        }


        foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
        {
            string valueString = ToDescriptionString<TEnum>(enumValue);
            if (valueString.Equals(value))
            {
                return enumValue;
            }
        }

        return default(TEnum);
    }

    public static bool IsFlagsEnum<TEnum>(this TEnum e)
        where TEnum : struct, Enum
    {
        return typeof(TEnum).GetCustomAttributes<FlagsAttribute>().Any();
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

    #region Type Dictionaries
    private static object _loadLock = new object();
    private static Dictionary<string, Type>? enums;

    public static bool TryGetEnumType(string fullTypeName, [MaybeNullWhen(false)] out Type t)
    {
        return LoadEnumTypes().TryGetValue(fullTypeName, out t);
    }

    private static Dictionary<string, Type> LoadEnumTypes()
    {
        lock (_loadLock)
        {
            if (enums != null) return enums;
        }
        enums = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
        {
            LoadEnumsFromAssembly(assemb, enums);
        }
        return enums;
    }

    private static void LoadEnumsFromAssembly(Assembly assembly, Dictionary<string, Type> dict)
    {
        try
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsEnum)
                {
                    dict[t.FullName!] = t;
                }
            }
        }
        catch (Exception)
        { }
    }
    #endregion

    #region Generic Bitwise Operators
    public static T And<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return EnumExt<T>.bitwiseOperations.Value.And(value1, value2);
    }
    public static T And<T>(IEnumerable<T> list)
        where T : struct, Enum
    {
        return list.Aggregate(And);
    }
    public static T Not<T>(this T value)
        where T : struct, Enum
    {
        return EnumExt<T>.bitwiseOperations.Value.Not(value);
    }
    public static T Or<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return EnumExt<T>.bitwiseOperations.Value.Or(value1, value2);
    }
    public static T Or<T>(IEnumerable<T> list)
        where T : struct, Enum
    {
        return list.Aggregate(Or);
    }
    public static T Xor<T>(this T value1, T value2)
        where T : struct, Enum
    {
        return EnumExt<T>.bitwiseOperations.Value.Xor(value1, value2);
    }
    public static T Xor<T>(IEnumerable<T> list)
        where T : struct, Enum
    {
        return list.Aggregate(Xor);
    }
    #endregion

    [Pure]
    public static bool HasFlag<T>(this T e, T toCheck)
        where T : struct, Enum
    {
        return e.HasFlag((Enum)toCheck);
    }

    [Pure]
    public static bool HasFlag(int value, int flagToCheck)
    {
        return (value & flagToCheck) > 0;
    }

    [Pure]
    public static bool HasFlag(uint value, uint flagToCheck)
    {
        return (value & flagToCheck) > 0;
    }

    [Pure]
    public static bool HasFlag(byte value, byte flagToCheck)
    {
        return (value & flagToCheck) > 0;
    }

    [Pure]
    public static int SetFlag(int origValue, int flagToSet, bool on)
    {
        if (on)
        {
            return origValue | flagToSet;
        }
        else
        {
            return origValue & ~flagToSet;
        }
    }

    public static void SetFlag(ref int origValue, int flagToSet, bool on)
    {
        origValue = SetFlag(origValue, flagToSet, on);
    }

    [Pure]
    public static uint SetFlag(uint origValue, uint flagToSet, bool on)
    {
        if (on)
        {
            return origValue | flagToSet;
        }
        else
        {
            return origValue & ~flagToSet;
        }
    }

    public static void SetFlag(ref uint origValue, uint flagToSet, bool on)
    {
        origValue = SetFlag(origValue, flagToSet, on);
    }

    [Pure]
    public static byte SetFlag(byte origValue, byte flagToSet, bool on)
    {
        if (on)
        {
            return (byte)(origValue | flagToSet);
        }
        else
        {
            return (byte)(origValue & ~flagToSet);
        }
    }

    public static void SetFlag(ref byte origValue, byte flagToSet, bool on)
    {
        origValue = SetFlag(origValue, flagToSet, on);
    }

    public static IEnumerable<TEnum> EnumerateContainedFlags<TEnum>(this TEnum flags, bool includeUndefined = true)
        where TEnum : struct, Enum
    {
        return EnumExt<TEnum>.EnumerateContainedFlags(flags, includeUndefined: includeUndefined);
    }
}

public static class EnumExt<T>
    where T : struct, Enum
{
    internal static Lazy<GenericBitwise<T>> bitwiseOperations = new(); 
    private static Lazy<T[]> _Values = new(() =>
    {
        List<T> ret = new List<T>();
        foreach (T item in Enum.GetValues(typeof(T)))
        {
            ret.Add(item);
        }
        return ret.ToArray();
    });
    public static T[] Values => _Values.Value;

    private static readonly Lazy<Func<long, T>> _convert = new(GenerateConverter);
    public static Func<long, T> Convert => _convert.Value;
    static Func<long, T> GenerateConverter()
    {
        var parameter = Expression.Parameter(typeof(long));
        var dynamicMethod = Expression.Lambda<Func<long, T>>(
            Expression.Convert(parameter, typeof(T)),
            parameter);
        return dynamicMethod.Compile();
    }

    private static readonly Lazy<Func<T, long>> _convertFrom = new(GenerateFromConverter);
    public static Func<T, long> ConvertFrom => _convertFrom.Value;
    static Func<T, long> GenerateFromConverter()
    {
        var parameter = Expression.Parameter(typeof(T));
        var dynamicMethod = Expression.Lambda<Func<T, long>>(
            Expression.Convert(parameter, typeof(long)),
            parameter);
        return dynamicMethod.Compile();
    }

    public static IEnumerable<T> EnumerateContainedFlags(T flags, bool includeUndefined = true)
    {
        if (includeUndefined)
        {
            return EnumFlags<T>.EnumerateAllContainedFlags(flags);
        }
        else
        {
            return EnumFlags<T>.EnumerateContainedDefinedFlags(flags);
        }
    }

    public static bool IsFlags => EnumFlags<T>._hasFlags;
}

static class EnumFlags<TEnum>
    where TEnum : struct, Enum
{
    public static bool _hasFlags;
    public static IReadOnlyList<TEnum> _flagValues;
    public static readonly int Bits = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(TEnum))) * 8;
    public static readonly uint MaxSize = (uint)(Math.Pow(2, Bits) - 1);

    static EnumFlags()
    {
        _hasFlags = IsFlagsEnum();
        _flagValues = GetFlagValues().ToArray();
    }

    public static bool IsFlagsEnum()
    {
        return typeof(TEnum).GetCustomAttributes<FlagsAttribute>().Any();
    }

    private static IEnumerable<TEnum> GetFlagValues()
    {
        if (!_hasFlags) yield break;
        ulong flag = 0x1;
        foreach (var value in EnumExt<TEnum>.Values)
        {
            ulong bits = Convert.ToUInt64(value);
            if (bits == 0L)
                //yield return value;
                continue; // skip the zero value
            while (flag < bits) flag <<= 1;
            if (flag == bits)
                yield return value;
        }
    }
    
    public static IEnumerable<TEnum> EnumerateContainedDefinedFlags(TEnum value)
    {
        if (!_hasFlags)
        {
            if (Enum.IsDefined(typeof(TEnum), value))
            {
                return new SingleCollection<TEnum>(value);
            }
            else
            {
                return Array.Empty<TEnum>();
            }
        }
        ulong bits = Convert.ToUInt64(value);
        var results = new List<TEnum>();
        for (int i = _flagValues.Count - 1; i >= 0; i--)
        {
            ulong mask = Convert.ToUInt64(_flagValues[i]);
            if (i == 0 && mask == 0L)
                break;
            if ((bits & mask) == mask)
            {
                results.Add(_flagValues[i]);
                bits -= mask;
            }
        }
        if (Convert.ToUInt64(value) != 0L)
            return results.Reverse<TEnum>();
        if (bits == Convert.ToUInt64(value) && _flagValues.Count > 0 && Convert.ToUInt64(_flagValues[0]) == 0L)
            return _flagValues.Take(1);
        return Enumerable.Empty<TEnum>();
    }

    public static IEnumerable<TEnum> EnumerateAllContainedFlags(TEnum flags)
    {
        if (!_hasFlags)
        {
            yield return flags;
            yield break;
        }
        ulong flagsLong = System.Convert.ToUInt64(flags);
        for (ulong i = 1; ; i <<= 1)
        {
            if ((flagsLong & i) > 0)
            {
                yield return (TEnum)Enum.ToObject(typeof(TEnum), i);
            }
            if (i >= MaxSize) break;
        }
    }
}

static class EnumStrings<T> where T : struct, Enum
{
    private static Dictionary<int, string> _strings;
    static EnumStrings()
    {
        if (typeof(T).IsEnum)
        {
            _strings = new Dictionary<int, string>();
            foreach (Enum value in Enum.GetValues(typeof(T)))
            {
                _strings[((IConvertible)value).ToInt32(CultureInfo.InvariantCulture)] = value.ToString();
            }
            EnumExt.NameDictionary[typeof(T)] = _strings;
        }
        else
        {
            throw new Exception("Generic type must be an enumeration");
        }
    }

    public static string GetEnumString(int enumValue)
    {
        return _strings[enumValue];
    }

    public static bool TryGetEnumString(int enumValue, [MaybeNullWhen(false)] out string str)
    {
        return _strings.TryGetValue(enumValue, out str);
    }
}

// https://stackoverflow.com/questions/53636974/c-sharp-method-to-combine-a-generic-list-of-enum-values-to-a-single-value
class GenericBitwise<TFlagEnum> where TFlagEnum : Enum
{
    private readonly Func<TFlagEnum, TFlagEnum, TFlagEnum> _and;
    private readonly Func<TFlagEnum, TFlagEnum> _not;
    private readonly Func<TFlagEnum, TFlagEnum, TFlagEnum> _or;
    private readonly Func<TFlagEnum, TFlagEnum, TFlagEnum> _xor;

    public GenericBitwise()
    {
        _and = And().Compile();
        _not = Not().Compile();
        _or = Or().Compile();
        _xor = Xor().Compile();
    }

    public TFlagEnum And(TFlagEnum value1, TFlagEnum value2) => _and(value1, value2);
    public TFlagEnum And(IEnumerable<TFlagEnum> list) => list.Aggregate(And);
    public TFlagEnum Not(TFlagEnum value) => _not(value);
    public TFlagEnum Or(TFlagEnum value1, TFlagEnum value2) => _or(value1, value2);
    public TFlagEnum Or(IEnumerable<TFlagEnum> list) => list.Aggregate(Or);
    public TFlagEnum Xor(TFlagEnum value1, TFlagEnum value2) => _xor(value1, value2);
    public TFlagEnum Xor(IEnumerable<TFlagEnum> list) => list.Aggregate(Xor);

    public TFlagEnum All()
    {
        var allFlags = Enum.GetValues(typeof(TFlagEnum)).Cast<TFlagEnum>();
        return Or(allFlags);
    }

    private Expression<Func<TFlagEnum, TFlagEnum>> Not()
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(TFlagEnum));
        var v1 = Expression.Parameter(typeof(TFlagEnum));

        return Expression.Lambda<Func<TFlagEnum, TFlagEnum>>(
            Expression.Convert(
                Expression.Not( // ~
                    Expression.Convert(v1, underlyingType)
                ),
                typeof(TFlagEnum) // convert the result of the tilde back into the enum type
            ),
            v1 // the argument of the function
        );
    }

    private Expression<Func<TFlagEnum, TFlagEnum, TFlagEnum>> And()
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(TFlagEnum));
        var v1 = Expression.Parameter(typeof(TFlagEnum));
        var v2 = Expression.Parameter(typeof(TFlagEnum));

        return Expression.Lambda<Func<TFlagEnum, TFlagEnum, TFlagEnum>>(
            Expression.Convert(
                Expression.And( // combine the flags with an AND
                    Expression.Convert(v1, underlyingType), // convert the values to a bit maskable type (i.e. the underlying numeric type of the enum)
                    Expression.Convert(v2, underlyingType)
                ),
                typeof(TFlagEnum) // convert the result of the AND back into the enum type
            ),
            v1, // the first argument of the function
            v2 // the second argument of the function
        );
    }

    private Expression<Func<TFlagEnum, TFlagEnum, TFlagEnum>> Or()
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(TFlagEnum));
        var v1 = Expression.Parameter(typeof(TFlagEnum));
        var v2 = Expression.Parameter(typeof(TFlagEnum));

        return Expression.Lambda<Func<TFlagEnum, TFlagEnum, TFlagEnum>>(
            Expression.Convert(
                Expression.Or( // combine the flags with an OR
                    Expression.Convert(v1, underlyingType), // convert the values to a bit maskable type (i.e. the underlying numeric type of the enum)
                    Expression.Convert(v2, underlyingType)
                ),
                typeof(TFlagEnum) // convert the result of the OR back into the enum type
            ),
            v1, // the first argument of the function
            v2 // the second argument of the function
        );
    }

    private Expression<Func<TFlagEnum, TFlagEnum, TFlagEnum>> Xor()
    {
        Type underlyingType = Enum.GetUnderlyingType(typeof(TFlagEnum));
        var v1 = Expression.Parameter(typeof(TFlagEnum));
        var v2 = Expression.Parameter(typeof(TFlagEnum));

        return Expression.Lambda<Func<TFlagEnum, TFlagEnum, TFlagEnum>>(
            Expression.Convert(
                Expression.ExclusiveOr( // combine the flags with an XOR
                    Expression.Convert(v1, underlyingType), // convert the values to a bit maskable type (i.e. the underlying numeric type of the enum)
                    Expression.Convert(v2, underlyingType)
                ),
                typeof(TFlagEnum) // convert the result of the OR back into the enum type
            ),
            v1, // the first argument of the function
            v2 // the second argument of the function
        );
    }
}