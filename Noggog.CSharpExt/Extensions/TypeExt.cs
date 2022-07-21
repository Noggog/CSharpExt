using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Noggog;

public static class TypeExt
{
    public static bool InheritsFrom(this Type t, Type baseType, bool excludeSelf = false, bool couldInherit = false)
    {
        if (baseType == t) return !excludeSelf;
        if (baseType.IsAssignableFrom(t)) return true;
        if (baseType.IsGenericType)
        {
            if (IsAssignableToGenericType(t, baseType, couldInherit: couldInherit)) return true;
        }
        if (couldInherit
            && baseType.IsGenericParameter
            && baseType.BaseType != null)
        {
            return t.InheritsFrom(baseType.BaseType, excludeSelf: excludeSelf, couldInherit: couldInherit);
        }
        return false;
    }

    public static bool IsAssignableToGenericType(Type givenType, Type genericType, bool couldInherit = false)
    {
        var genTypeDef = genericType.GetGenericTypeDefinition();
        foreach (var it in givenType.GetInterfaces())
        {
            if (!it.IsGenericType) continue;
            var genDef = it.GetGenericTypeDefinition();
            if (!genDef.Equals(genTypeDef)) continue;
            if (it.GenericTypeArguments.Length != genericType.GenericTypeArguments.Length) return false;
            for (int i = 0; i < it.GenericTypeArguments.Length; i++)
            {
                if (!it.GenericTypeArguments[i].InheritsFrom(genericType.GenericTypeArguments[i])) return false;
            }
            return true;
        }

        if (givenType.IsGenericType)
        {
            if (givenType.GetGenericTypeDefinition() == genericType) return true;
            if (givenType.GenericTypeArguments.Length != genericType.GenericTypeArguments.Length
                || givenType.GenericTypeArguments.Length == 0) return false;
            for (int i = 0; i < givenType.GenericTypeArguments.Length; i++)
            {
                var genType = genericType.GenericTypeArguments[i];
                if (!givenType.GenericTypeArguments[i].InheritsFrom(genType))
                {
                    if (!couldInherit) return false;
                    if (!genType.IsGenericParameter) return false;
                    if (!givenType.GenericTypeArguments[i].InheritsFrom(genType.BaseType!, excludeSelf: false, couldInherit: true)) return false;
                }
            }
            return true;
        }

        Type? baseType = givenType.BaseType;
        if (baseType == null) return false;

        return IsAssignableToGenericType(baseType, genericType);
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

        if ("Nullable".Equals(name, StringComparison.CurrentCultureIgnoreCase))
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
            toCheck = toCheck.BaseType!;
        }
        return false;
    }

    public static IEnumerable<Type> GetSubclassesOf(this Type baseType)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && !p.IsInterface);
    }

    public static List<string>? FailedAssemblyLoads;

    public static IEnumerable<Type> GetInheritingFromInterface<I>(bool loadAssemblies = true, Func<Assembly, bool>? filter = null)
    {
        return GetInheritingFromInterface(typeof(I), loadAssemblies, filter);
    }

    public static IEnumerable<KeyValuePair<Type, Type>> GetInheritingFromGenericInterface(this Type targetType, bool loadAssemblies = true, Func<Assembly, bool>? filter = null)
    {
        foreach (var type in IterateTypes(loadAssemblies: loadAssemblies, assemblyFilter: filter))
        {
            if (type.Equals(targetType)) continue;

            foreach (var i in type.GetInterfaces())
            {
                if (!i.IsGenericType) continue;
                if (targetType.Equals(i.GetGenericTypeDefinition()))
                {
                    yield return new KeyValuePair<Type, Type>(i, type);
                }
            }
        }
    }

    public static IEnumerable<Type> GetInheritingFromInterface(this Type targetType, bool loadAssemblies = true, Func<Assembly, bool>? filter = null)
    {
        foreach (var type in IterateTypes(loadAssemblies: loadAssemblies, assemblyFilter: filter))
        {
            if (type.Equals(targetType))
                continue;

            if (targetType.IsGenericType)
            {
                if (!type.IsGenericType)
                    continue;
                foreach (var i in type.GetInterfaces())
                {
                    if (!i.IsGenericType)
                        continue;
                    if (targetType.Equals(i.GetGenericTypeDefinition()))
                    {
                        yield return type;
                    }
                }
            }
            else
            {
                if (targetType.IsAssignableFrom(type))
                {
                    yield return type;
                }
            }
        }
    }

    public static IEnumerable<Type> IterateTypes(bool loadAssemblies = true, Func<Assembly, bool>? assemblyFilter = null)
    {
        if (loadAssemblies)
        {
            LoadAssemblies();
        }
        foreach (Assembly assemb in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assemblyFilter != null && !assemblyFilter(assemb)) continue;
            Type[] types;
            try
            {
                types = assemb.GetTypes();
            }
            catch (ReflectionTypeLoadException)
            {
                continue;
            }
            foreach (Type type in types)
            {
                yield return type;
            }
        }
    }

    public static void LoadAssemblies()
    {
        LoadAssemblies(out List<string> _);
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
            catch (FileNotFoundException)
            {
                FailedAssemblyLoads.Add(path);
            }
        });
        failed = FailedAssemblyLoads;
    }

    /*
     * Not cheap
     */
    public static Type? FindType(string qualifiedTypeName)
    {
        Type? t = Type.GetType(qualifiedTypeName);

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

    public static bool TryGetEnumerableType(this Type t, [MaybeNullWhen(false)] out Type enumerType)
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

    private static bool IsNullable_Internal(Type type, [MaybeNullWhen(false)] out Type underlying)
    {
        if (!type.IsValueType)
        {
            underlying = type;
            return true;
        }
        underlying = Nullable.GetUnderlyingType(type);
        if (underlying != null) return true;
        return false;
    }

    public static bool IsNullable<T>()
    {
        return IsNullable_Internal(typeof(T), out var _);
    }

    public static bool IsNullable(Type t)
    {
        return IsNullable_Internal(t, out var _);
    }

    public static bool IsNullable<T>([MaybeNullWhen(false)] out Type underlying)
    {
        return IsNullable_Internal(typeof(T), out underlying);
    }

    public static bool IsNullable<T>(Type t, [MaybeNullWhen(false)] out Type underlying)
    {
        return IsNullable_Internal(t, out underlying);
    }

    /// <summary>
    /// Helps to get properties in inherited interfaces
    /// </summary>
    public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
    {
        if (!type.IsInterface)
            return type.GetProperties();

        return (new Type[] { type })
            .Concat(type.GetInterfaces())
            .SelectMany(i => i.GetProperties());
    }
}