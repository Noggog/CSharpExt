using Noggog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class EnumExt
    {
        /**
        * Includes an enumerated type and returns the new value.
        */
        public static T Include<T>(this Enum value, T append)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(append, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /**
         * Added these for the sake of not needing to do an assignation.
         */
        public static void Add<T>(this Enum append, ref T value)
        {
            value = Include(append, value);
        }

        public static void Remove<T>(this Enum remove, ref T value)
        {
            value = Expel(remove, value);
        }

        /**
        * Takes out an enumerated type and returns the new value.
        */
        public static T Expel<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            _Value parsed = new _Value(remove, type);
            if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /**
        * Checks if an enumerated type contains a value.
        */
        public static bool Has<T>(this Enum value, T check)
        {
            Type type = value.GetType();

            //determine the values
            _Value parsed = new _Value(check, type);
            if (parsed.Signed is long)
            {
                return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;
            }
            else if (parsed.Unsigned is ulong)
            {
                return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;
            }
            else
            {
                return false;
            }
        }

        #region Helper Classes

        //class to simplfy narrowing values between 
        //a ulong and long since either value should
        //cover any lesser value
        private class _Value
        {
            //cached comparisons for type to use
            private static Type _UInt64 = typeof(ulong);
            private static Type _UInt32 = typeof(long);

            public long? Signed;
            public ulong? Unsigned;

            public _Value(object value, Type type)
            {
                //make sure it is even an enum to work with
                if (!type.IsEnum)
                {
                    throw new ArgumentException("Value provided is not an enumerated type!");
                }

                //then check for the enumerated value
                Type compare = Enum.GetUnderlyingType(type);

                //if this is an unsigned long then the only
                //value that can hold it would be a ulong
                if (compare.Equals(_Value._UInt32) || compare.Equals(_Value._UInt64))
                {
                    this.Unsigned = Convert.ToUInt64(value);
                }
                //otherwise, a long should cover anything else
                else
                {
                    this.Signed = Convert.ToInt64(value);
                }
            }
        }
        #endregion

        public static int Length(this Enum obj)
        {
            return Enum.GetValues(obj.GetType()).Length;
        }

        public static int Length<T>()
            where T : struct, IComparable, IConvertible
        {
            return Length((Enum)Activator.CreateInstance(typeof(T)));
        }

        public static bool TryParse<T>(int number, out T val)
        {
            if (Enum.IsDefined(typeof(T), number))
            {
                val = (T)(object)number;
                return true;
            }

            val = default(T);
            return false;
        }

        public static T Parse<T>(int number, T defaultTo)
            where T : struct, IComparable, IConvertible
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
            where T : struct, IComparable, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T[] GetValueArray<T>()
            where T : struct, IComparable, IConvertible
        {
            return GetValues<T>().ToArray<T>();
        }

        public static int GetSize<T>()
            where T : struct, IComparable, IConvertible
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
            where T : struct, IComparable, IConvertible
        {
            return GetNth<T>(n, default(T));
        }

        public static T GetNth<T>(int n, T defaultPick)
            where T : struct, IComparable, IConvertible
        {
            string[] s = Enum.GetNames(typeof(T));
            if (s.Length <= n) return defaultPick;
            return (T)Enum.Parse(typeof(T), s[n]);
        }

        public static Dictionary<Type, Dictionary<int, string>> NameDictionary = new Dictionary<Type, Dictionary<int, string>>();

        // Slower
        public static string ToStringFast_Enum_Only<T>(this T e)
            where T : struct, IComparable, IConvertible
        {
            IConvertible cv = (IConvertible)e;
            return EnumStrings<T>.GetEnumString(cv.ToInt32(CultureInfo.InvariantCulture));
        }

        // Faster
        public static string ToStringFast_Enum_Only<T>(int enumVal)
            where T : struct, IComparable, IConvertible
        {
            return EnumStrings<T>.GetEnumString(enumVal);
        }

        public static string ToStringFast_Enum_Only(Type enumType, int index)
        {
            if (!NameDictionary.TryGetValue(enumType, out Dictionary<int, string> arr))
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

        public static string ToDescriptionString<TEnum>(this TEnum val) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("T must be an Enum");
            }

            DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        public static TEnum FromDescriptionString<TEnum>(string value, TEnum defaultValue = default(TEnum)) where TEnum : struct, IConvertible
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
                string valueString = EnumExt.ToDescriptionString<TEnum>(enumValue);
                if (valueString.Equals(value))
                {
                    return enumValue;
                }
            }

            return default(TEnum);
        }

        #region Type Dictionaries
        private static object _loadLock = new object();
        private static Dictionary<StringCaseAgnostic, Type> enums;

        public static bool TryGetEnumType(string fullTypeName, out Type t)
        {
            LoadEnumTypes();
            return enums.TryGetValue(fullTypeName, out t);
        }

        private static void LoadEnumTypes()
        {
            lock (_loadLock)
            {
                if (enums != null) return;
            }
            enums = new Dictionary<StringCaseAgnostic, Type>();
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadEnumsFromAssembly(assemb);
            }
        }

        private static void LoadEnumsFromAssembly(Assembly assembly)
        {
            try
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.IsEnum)
                    {
                        enums[t.FullName] = t;
                    }
                }
            }
            catch (Exception)
            { }
        }
        #endregion
    }

    static class EnumStrings<T> where T : struct, IComparable, IConvertible
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
    }
}