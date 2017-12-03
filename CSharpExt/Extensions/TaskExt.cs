using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class TaskExt
    {
        public static async Task<T> AwaitOrDefault<T>(Task<T> t)
        {
            if (t == null) return default(T);
            return await t;
        }

        public static async Task<T> AwaitOrDefaultValue<T>(Task<TryGet<T>> t)
        {
            if (t == null) return default(T);
            return (await t).Value;
        }
    }
}
