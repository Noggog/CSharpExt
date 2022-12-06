using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Noggog;

public static class Enums
{
    public static bool TryGetNth(Type t, int n, [MaybeNullWhen(false)] out Enum e)
    {
        string[] s = Enum.GetNames(t);
        if (s.Length <= n || n < 0)
        {
            e = default!;
            return false;
        }
        e = (Enum)Enum.Parse(t, s[n]);
        return true;
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
            enums = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase); 
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies()) 
            { 
                LoadEnumsFromAssembly(assemb, enums); 
            } 
            return enums; 
        } 
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
}

public static class Enums<TEnum>
    where TEnum : struct, Enum
{
    internal static Lazy<GenericBitwise<TEnum>> bitwiseOperations = new(); 
    private static Lazy<TEnum[]> _Values = new(() =>
    {
        var ret = new List<TEnum>();
        foreach (TEnum item in Enum.GetValues(typeof(TEnum)))
        {
            ret.Add(item);
        }
        return ret.ToArray();
    });
    public static IReadOnlyList<TEnum> Values => _Values.Value;
    private static Lazy<HashSet<TEnum>> _valueSet = new(() => new HashSet<TEnum>(Values));
    public static bool IsDefined(TEnum e) => _valueSet.Value.Contains(e);
    
    // ToDo
    // Avoid allocation
    public static bool IsDefined(int e) => Enum.IsDefined(typeof(TEnum), e);
    // ToDo
    // Avoid allocation
    public static bool IsDefined(long e) => Enum.IsDefined(typeof(TEnum), e);

    private static readonly Lazy<Func<long, TEnum>> _convert = new(GenerateConverter);
    private static Func<long, TEnum> GenerateConverter()
    {
        var parameter = Expression.Parameter(typeof(long));
        var dynamicMethod = Expression.Lambda<Func<long, TEnum>>(
            Expression.Convert(parameter, typeof(TEnum)),
            parameter);
        return dynamicMethod.Compile();
    }

    private static readonly Lazy<Func<TEnum, long>> _convertFrom = new(GenerateFromConverter);
    public static Func<TEnum, long> ConvertFrom => _convertFrom.Value;
    static Func<TEnum, long> GenerateFromConverter()
    {
        var parameter = Expression.Parameter(typeof(TEnum));
        var dynamicMethod = Expression.Lambda<Func<TEnum, long>>(
            Expression.Convert(parameter, typeof(long)),
            parameter);
        return dynamicMethod.Compile();
    }

    public static IEnumerable<TEnum> EnumerateContainedFlags(TEnum flags, bool includeUndefined = true)
    {
        if (includeUndefined)
        {
            return EnumFlags<TEnum>.EnumerateAllContainedFlags(flags);
        }
        else
        {
            return EnumFlags<TEnum>.EnumerateContainedDefinedFlags(flags);
        }
    }

    public static bool IsFlagsEnum => EnumFlags<TEnum>.IsFlagsEnum;

    public static int NumEntries { get; } = Enum.GetValues(typeof(TEnum)).Length;

    public static bool TryConvert(int number, out TEnum val)
    {
        if (IsDefined(number))
        {
            val = Convert(number);
            return true;
        }

        val = default;
        return false;
    }

    public static TEnum TryConvert(int number, TEnum defaultTo)
    {
        if (IsDefined(number))
        {
            return Convert(number);
        }
        else
        {
            return defaultTo;
        }
    }

    public static TEnum Convert(long number)
    {
        return _convert.Value(number);
    }

    public static bool TryConvert(long number, out TEnum val)
    {
        if (IsDefined(number))
        {
            val = Convert(number);
            return true;
        }

        val = default;
        return false;
    }

    public static TEnum TryConvert(long number, TEnum defaultTo)
    {
        if (IsDefined(number))
        {
            return Convert(number);
        }
        else
        {
            return defaultTo;
        }
    }

#if NETSTANDARD2_0 
#else
    public static bool TryParseFromNumber(ReadOnlySpan<char> str, out TEnum e)
    {
        if (Enum.TryParse<TEnum>(str, out var parsedEnum))
        {
            e = parsedEnum;
            return true;
        }

        var isNeg = str.StartsWith("-");
        if (isNeg)
        {
            str = str.Slice(1);
        }

        NumberStyles style = NumberStyles.Integer;
        if (str.StartsWith("0x"))
        {
            style = NumberStyles.HexNumber;
            str = str.Slice(2);
        }
                
        if (long.TryParse(str, style, null, out var l))
        {
            if (isNeg)
            {
                l *= -1;
            }
            e = Convert(l);
            return true;
        }
        
        if (ulong.TryParse(str, style, null, out var ul))
        {
            if (isNeg)
            {
                e = default;
                return false;
            }
            e = Convert((long)ul);
            return true;
        }

        e = default;
        return false;
    }

    public static TEnum TryParseFromNumber(ReadOnlySpan<char> str)
    {
        if (TryParseFromNumber(str, out var e)) return e;
        throw new ArgumentException($"Could not be converted to {typeof(TEnum)}: {str}", nameof(str));
    }
#endif

    public static TEnum Convert(int number)
    {
        return _convert.Value(number);
    }

    public static TEnum GetNth(int n, TEnum defaultPick = default(TEnum))
    {
        Array s = Enum.GetValues(typeof(TEnum));
        if (s.Length <= n || n < 0) return defaultPick;
        return (TEnum)s.GetValue(n)!;
    }

    public static bool TryGetNth(int n, [MaybeNullWhen(false)] out TEnum e)
    {
        var arr = Enum.GetValues(typeof(TEnum));
        if (arr.Length <= n || n < 0)
        {
            e = default!;
            return false;
        }

        e = (TEnum)arr.GetValue(n)!;
        return true;
    }

    public static string ToStringFast(int enumVal)
    {
        if (EnumStrings<TEnum>.TryGetEnumString(enumVal, out var str))
        {
            return str;
        }

        if (EnumFlags<TEnum>.IsFlagsEnum)
        {
            return $"0x{enumVal:X}";
        }
        else
        {
            return enumVal.ToString();
        }
    }

    public static bool TryToStringFast(int enumVal, [MaybeNullWhen(false)] out string str)
    {
        return EnumStrings<TEnum>.TryGetEnumString(enumVal, out str);
    }

    // ToDo
    // Cache
    public static string ToDescriptionString(TEnum val)
    {
        DescriptionAttribute[] attributes = (DescriptionAttribute[])typeof(TEnum).GetField(val.ToString())!.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : string.Empty;
    }

    #region Generic Bitwise Operators
    public static TEnum And(TEnum value1, TEnum value2)
    {
        return bitwiseOperations.Value.And(value1, value2);
    }
    
    public static TEnum And(IEnumerable<TEnum> list)
    {
        return list.Aggregate(And);
    }
    
    public static TEnum Not(TEnum value)
    {
        return bitwiseOperations.Value.Not(value);
    }
    
    public static TEnum Or(TEnum value1, TEnum value2)
    {
        return bitwiseOperations.Value.Or(value1, value2);
    }
    
    public static TEnum Or(IEnumerable<TEnum> list)
    {
        return list.Aggregate(Or);
    }
    
    public static TEnum Xor(TEnum value1, TEnum value2)
    {
        return bitwiseOperations.Value.Xor(value1, value2);
    }
    
    public static TEnum Xor(IEnumerable<TEnum> list)
    {
        return list.Aggregate(Xor);
    }
    #endregion

    [Pure]
    public static bool HasFlag(TEnum e, TEnum toCheck)
    {
        return e.HasFlag(toCheck);
    }
    
    internal static readonly UnderlyingType? Type = GetUnderlyingType();
    
    internal enum UnderlyingType
    {
        Int8,
        Int16,
        Int32,
        Int64,
        UInt8,
        UInt16,
        UInt32,
        UInt64,
    }

    internal static UnderlyingType? GetUnderlyingType()
    {
        var t = Enum.GetUnderlyingType(typeof(TEnum));
        if (t == typeof(byte))
        {
            return UnderlyingType.UInt8;
        }
        if (t == typeof(ushort))
        {
            return UnderlyingType.UInt16;
        }
        if (t == typeof(uint))
        {
            return UnderlyingType.UInt32;
        }
        if (t == typeof(ulong))
        {
            return UnderlyingType.UInt64;
        }
        if (t == typeof(sbyte))
        {
            return UnderlyingType.Int8;
        }
        if (t == typeof(short))
        {
            return UnderlyingType.Int16;
        }
        if (t == typeof(int))
        {
            return UnderlyingType.Int32;
        }
        if (t == typeof(long))
        {
            return UnderlyingType.Int64;
        }
    
        return null;
    }
}

static class EnumFlags<TEnum>
    where TEnum : struct, Enum
{
    public static bool IsFlagsEnum;
    public static IReadOnlyList<TEnum> FlagValues;
    public static readonly int Bits = Marshal.SizeOf(Enum.GetUnderlyingType(typeof(TEnum))) * 8;
    public static readonly uint MaxSize = (uint)(Math.Pow(2, Bits) - 1);

    static EnumFlags()
    {
        IsFlagsEnum = typeof(TEnum).GetCustomAttributes<FlagsAttribute>().Any();
        FlagValues = GetFlagValues().ToArray();
    }

    private static IEnumerable<TEnum> GetFlagValues()
    {
        if (!IsFlagsEnum) yield break;
        ulong flag = 0x1;
        foreach (var value in Enums<TEnum>.Values)
        {
            var bits = (ulong)Enums<TEnum>.ConvertFrom(value);
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
        if (!IsFlagsEnum)
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
        ulong bits = (ulong)Enums<TEnum>.ConvertFrom(value);
        var results = new List<TEnum>();
        for (int i = FlagValues.Count - 1; i >= 0; i--)
        {
            var mask = (ulong)Enums<TEnum>.ConvertFrom(FlagValues[i]);
            if (i == 0 && mask == 0L)
                break;
            if ((bits & mask) == mask)
            {
                results.Add(FlagValues[i]);
                bits -= mask;
            }
        }
        if (Enums<TEnum>.ConvertFrom(value) != 0L)
            return results.Reverse<TEnum>();
        if (bits == (ulong)Enums<TEnum>.ConvertFrom(value) && FlagValues.Count > 0 && Enums<TEnum>.ConvertFrom(FlagValues[0]) == 0L)
            return FlagValues.Take(1);
        return Enumerable.Empty<TEnum>();
    }

    public static IEnumerable<TEnum> EnumerateAllContainedFlags(TEnum flags)
    {
        if (!IsFlagsEnum)
        {
            yield return flags;
            yield break;
        }
        ulong flagsLong = (ulong)Enums<TEnum>.ConvertFrom(flags);
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

static class EnumStrings<T> 
    where T : struct, Enum
{
    private static readonly IReadOnlyDictionary<T, string> _stringsByVal;
    private static readonly Func<T, string> _backupStringCall;
    
    static EnumStrings()
    {
        var stringsByInt = new Dictionary<long, string>();
        var stringsByval = new Dictionary<T, string>();
        foreach (var value in Enums<T>.Values)
        {
            var str = value.ToString();
            stringsByval[value] = str;
        }

        _stringsByVal = stringsByval;
            
            
        switch (Enums<T>.Type)
        {
            case Enums<T>.UnderlyingType.Int8:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, sbyte>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, sbyte>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.Int16:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, short>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, short>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.Int32:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, int>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, int>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.Int64:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, long>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, long>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.UInt8:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, byte>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, byte>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.UInt16:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, ushort>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, ushort>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.UInt32:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, uint>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, uint>.Converter(t).ToString();
                }
                break;
            case Enums<T>.UnderlyingType.UInt64:
                if (EnumFlags<T>.IsFlagsEnum)
                {
                    _backupStringCall = static (t) => $"0x{EnumConvertToIntHelper<T, ulong>.Converter(t):X}";
                }
                else
                {
                    _backupStringCall = static (t) => EnumConvertToIntHelper<T, ulong>.Converter(t).ToString();
                }
                break;
            case null:
                _backupStringCall = static (t) => t.ToString();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public static string GetEnumString(long enumValue)
    {
        return _stringsByVal[Enums<T>.Convert(enumValue)];
    }

    public static bool TryGetEnumString(long enumValue, [MaybeNullWhen(false)] out string str)
    {
        return _stringsByVal.TryGetValue(Enums<T>.Convert(enumValue), out str);
    }

    public static bool TryGetEnumString(T enumValue, [MaybeNullWhen(false)] out string str)
    {
        return _stringsByVal.TryGetValue(enumValue, out str);
    }

    public static string GetEnumString(T e)
    {
        if (TryGetEnumString(e, out var str))
        {
            return str;
        }

        return _backupStringCall(e);
    }
}

internal static class EnumConvertToIntHelper<T, TIntType>
    where T : struct, Enum
{
    public static Func<T, TIntType> Converter = GenerateFunc<T>();

    private static Func<TEnum, TIntType> GenerateFunc<TEnum>()
        where TEnum : struct, Enum
    {
        var inputParameter = Expression.Parameter(typeof(TEnum));

        var body = Expression.Convert(inputParameter, typeof(TIntType));

        var lambda = Expression.Lambda<Func<TEnum, TIntType>>(body, inputParameter);

        var func = lambda.Compile();

        return func;
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