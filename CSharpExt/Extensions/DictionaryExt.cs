using System;
using System.Collections.Generic;

namespace System
{
    public static class DictionaryExt
    {
        public static V TryCreateValue<K, V>(this Dictionary<K, V> dict, K key)
            where V : new()
        {
            V ret;
            if (!dict.TryGetValue(key, out ret))
            {
                ret = new V();
                dict[key] = ret;
            }
            return ret;
        }

        public static V TryCreateValue<K, V>(this Dictionary<K, V> dict, K key, Func<V> getNew)
            where V : new()
        {
            V ret;
            if (!dict.TryGetValue(key, out ret))
            {
                ret = getNew();
                dict[key] = ret;
            }
            return ret;
        }
    }
}