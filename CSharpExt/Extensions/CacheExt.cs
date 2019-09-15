using DynamicData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Noggog
{
    public static class CacheExt
    {
        public static IEnumerable<KeyValuePair<K, R>> SelectAgainst<K, V, R>(
            this IReadOnlyCache<V, K> lhs,
            IReadOnlyCache<V, K> rhs,
            Func<K, V, V, R> selector,
            out bool equal)
        {
            List<KeyValuePair<K, R>> ret = new List<KeyValuePair<K, R>>();
            equal = lhs.Count == rhs.Count;
            foreach (var item in lhs)
            {
                if (!rhs.ContainsKey(item.Key))
                {
                    equal = false;
                    continue;
                }
                ret.Add(
                    new KeyValuePair<K, R>(
                        item.Key,
                        selector(item.Key, item.Value, rhs[item.Key])));
            }
            return ret;
        }
    }
}
