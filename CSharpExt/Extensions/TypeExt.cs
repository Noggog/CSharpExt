using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    public static class TypeExt
    {
        public static bool InheritsFrom(this Type t, Type baseType)
        {
            return baseType.IsAssignableFrom(t) && baseType != t;
        }

        public static string GetName(this Type t)
        {
            if (!t.IsGenericType)
            {
                return t.Name;
            }

            string name = t.Name;
            StringBuilder sb = new StringBuilder();
            int index = name.IndexOf("`");
            name = name.Substring(0, index);

            var genArgs = t.GetGenericArguments();

            if (name.EqualsIgnoreCase("Nullable"))
            {
                if (genArgs.Length > 1)
                {
                    throw new NotImplementedException();
                }
                sb.Append(genArgs[0].GetName());
                sb.Append("?");
                return sb.ToString();
            }

            sb.Append(name);
            sb.Append("<");
            bool first = true;
            foreach (Type gen in genArgs)
            {
                if (first)
                    first = false;
                else
                    sb.Append(", ");
                sb.Append(gen.GetName());
            }
            sb.Append(">");
            return sb.ToString();
        }

        public static bool IsPrimitive(this Type toCheck)
        {
            if (toCheck == typeof(String)) return true;
            return (toCheck.IsValueType && toCheck.IsPrimitive)
                || toCheck.IsEnum;
        }

        public static string GetSimpleName(this Type t)
        {
            string str = t.Name;
            int index = str.IndexOf("`");
            if (index != -1)
                return str.Substring(0, index);
            return str;
        }

        public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        public static IEnumerable<Type> GetSubclassesOf(this Type baseType)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && !p.IsInterface);
        }

        public static List<string> FailedAssemblyLoads;

        public static IEnumerable<Type> GetInheritingFromInterface<I>(bool loadAssemblies = true)
        {
            return GetInheritingFromInterface(typeof(I), loadAssemblies);
        }

        public static IEnumerable<KeyValuePair<Type, Type>> GetInheritingFromGenericInterface(this Type targetType, bool loadAssemblies = true)
        {
            if (loadAssemblies)
            {
                LoadAssemblies();
            }
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable<Type> types;
                try
                {
                    types = assemb.GetTypes();
                }
                catch (Exception ex)
                {
                    continue;
                }
                foreach (Type p in types)
                {
                    if (p.Equals(targetType)) continue;

                    foreach (var i in p.GetInterfaces())
                    {
                        if (!i.IsGenericType) continue;
                        if (targetType.Equals(i.GetGenericTypeDefinition()))
                        {
                            yield return new KeyValuePair<Type, Type>(i, p);
                            break;
                        }
                    }
                }
            }
        }

        public static IEnumerable<Type> GetInheritingFromInterface(this Type targetType, bool loadAssemblies = true)
        {
            if (loadAssemblies)
            {
                LoadAssemblies();
            }
            foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type p in assemb.GetTypes())
                {
                    if (p.Equals(targetType))
                        continue;

                    if (targetType.IsGenericType)
                    {
                        if (!p.IsGenericType)
                            continue;
                        foreach (var i in p.GetInterfaces())
                        {
                            if (!i.IsGenericType)
                                continue;
                            if (targetType.Equals(i.GetGenericTypeDefinition()))
                            {
                                yield return p;
                                continue;

                            }
                        }
                    }
                    else
                    {
                        if (targetType.IsAssignableFrom(p))
                        {
                            yield return p;
                            continue;
                        }
                    }
                }
            }
        }

        public static void LoadAssemblies()
        {
            LoadAssemblies(out List<string> failed);
        }

        public static void LoadAssemblies(out List<string> failed)
        {
            if (FailedAssemblyLoads != null)
            {
                failed = FailedAssemblyLoads;
                return;
            }
            FailedAssemblyLoads = new List<string>();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            List<string> loadedPaths = new List<string>();
            foreach (Assembly a in loadedAssemblies)
            {
                try
                {
                    loadedPaths.Add(a.Location);
                }
                catch (Exception)
                {
                    FailedAssemblyLoads.Add(a.ToString());
                }
            }

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path =>
            {
                try
                {
                    AssemblyName name = AssemblyName.GetAssemblyName(path);
                    Assembly assembly = AppDomain.CurrentDomain.Load(name);
                    loadedAssemblies.Add(assembly);
                }
                catch (BadImageFormatException)
                {
                    FailedAssemblyLoads.Add(path);
                }
            });
            failed = FailedAssemblyLoads;
        }

        /*
         * Not cheap
         */
        public static Type FindType(string qualifiedTypeName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    t = asm.GetType(qualifiedTypeName);
                    if (t != null)
                        return t;
                }
                return null;
            }
        }

        public static bool TryGetEnumerableType(this Type t, out Type enumerType)
        {
            foreach (Type interfType in t.GetInterfaces())
            {
                if (interfType.IsGenericType
                    && interfType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    enumerType = interfType.GetGenericArguments()[0];
                    return true;
                }
            }
            enumerType = null;
            return false;
        }
    }
}
