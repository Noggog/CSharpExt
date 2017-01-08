using System;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    public static class ObjectExt
    {
        public static Type GetEnumeratedType<T>(this IComparable<T> _)
        {
            return typeof(T);
        }

        public static bool InheritsFrom<T>(this object obj)
        {
            return obj is T;
        }

        public static List<T> FindAllDerivedObjects<T>(this Object obj, bool recursive = true)
        {
            List<T> ret = new List<T>();
            if (obj != null)
            {
                Type objType = obj.GetType();
                if (!objType.IsPrimitive())
                {
                    Type target = typeof(T);
                    var hashSet = new HashSet<object>(ReferenceEqualityComparer<object>.Instance);
                    hashSet.Add(obj);
                    if (target.IsAssignableFrom(objType))
                    {
                        ret.Add((T)obj);
                    }
                    ret.AddRange(FindAllDerivedObjects<T>(obj, obj.GetType(), target, hashSet, recursive));
                }
            }
            return ret;
        }

        private static List<T> FindAllDerivedObjects<T>(Object obj, Type objType, Type target, HashSet<Object> set, bool recursive)
        {
            List<T> ret = new List<T>(0);
            foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy))
            {
                Type fieldType = field.FieldType;
                if (!fieldType.IsPrimitive())
                {
                    object fieldObj = field.GetValue(obj);
                    if (fieldObj == null || !set.Add(fieldObj)) continue;
                    fieldType = fieldObj.GetType();
                    if (target.IsAssignableFrom(fieldType))
                    {
                        ret.Add((T)fieldObj);
                    }
                    if (fieldType.IsArray)
                    {
                        var arrayType = fieldType.GetElementType();
                        Array arrayObject = (Array)(fieldObj);
                        if (target.IsAssignableFrom(arrayType))
                        {
                            for (int i = 0; i < arrayObject.Length; ++i)
                            {
                                ret.Add((T)arrayObject.GetValue(i));
                            }
                        }
                        else
                        {
                            if (arrayObject.Rank == 1)
                            {
                                for (int i = 0; i < arrayObject.Length; ++i)
                                {
                                    var arrayItem = arrayObject.GetValue(i);
                                    if (arrayItem == null) continue;
                                    ret.AddRange(FindAllDerivedObjects<T>(arrayItem, arrayType, target, set, recursive));
                                }
                            }
                            else
                            {
                                for (int i = 0; i < arrayObject.GetLength(0); ++i)
                                {
                                    for (int j = 0; j < arrayObject.GetLength(1); j++)
                                    {
                                        var arrayItem = arrayObject.GetValue(i, j);
                                        if (arrayItem == null) continue;
                                        ret.AddRange(FindAllDerivedObjects<T>(arrayItem, arrayType, target, set, recursive));
                                    }
                                }
                            }
                        }
                    }
                    else if (recursive)
                    {
                        ret.AddRange(FindAllDerivedObjects<T>(fieldObj, fieldType, target, set, recursive));
                    }
                }
            }
            return ret;
        }

        public static string ToStringSafe(this object o, bool throwException = false)
        {
            if (o == null) return string.Empty;
            try
            {
                return o.ToString();
            }
            catch (Exception)
            {
                if (throwException)
                {
                    throw;
                }
                return "[ExceptionThrown]";
            }
        }

        public static R CastWithException<R>(this object o)
            where R : class
        {
            if (o == null) return null;
            R ret = o as R;
            if (ret == null)
            {
                throw new ArgumentException("Failed to cast from type " + o.GetType() + " to " + typeof(R));
            }
            return ret;
        }
    }
}
