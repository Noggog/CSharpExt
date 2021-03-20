using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class MemberInfoExt
    {
        public static bool TryGetCustomAttribute<T>(this MemberInfo memberInfo, [MaybeNullWhen(false)] out T attr)
            where T : Attribute
        {
            attr = memberInfo.GetCustomAttribute<T>();
            return attr != null;
        }

        public static bool IsStatic(this PropertyInfo memberInfo)
        {
            var method = (memberInfo.GetMethod ?? memberInfo.SetMethod)!;
            return method.IsStatic;
        }
    }
}
