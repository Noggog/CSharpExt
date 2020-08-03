using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    public static class DictionaryExt
    {
        public static void Set<K, V>(this IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> items)
        {
            foreach (var item in items)
            {
                dict[item.Key] = item.Value;
            }
        }

        public static V GetOrAdd<K, V>(this IDictionary<K, V> dict, K key)
            where V : new()
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = new V();
                dict[key] = ret;
            }
            return ret;
        }

        public static V GetOrAdd<K, V>(this IDictionary<K, V> dict, K key, Func<V> getNew)
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = getNew();
                dict[key] = ret;
            }
            return ret;
        }
        
        public static IEnumerable<KeyValuePair<K, R>> SelectAgainst<K, V, R>(
            this IReadOnlyDictionary<K, V> lhs, 
            IReadOnlyDictionary<K, V> rhs, 
            Func<K, V, V, R> selector, 
            out bool equal)
        {
            List<KeyValuePair<K, R>> ret = new List<KeyValuePair<K, R>>();
            equal = lhs.Count == rhs.Count;
            foreach (var item in lhs)
            {
                if (!rhs.TryGetValue(item.Key, out var rhsItem))
                {
                    equal = false;
                    continue;
                }
                ret.Add(
                    new KeyValuePair<K, R>(
                        item.Key,
                        selector(item.Key, item.Value, rhsItem)));
            }
            return ret;
        }

        public static void Remove<K, V>(this IDictionary<K, V> dict, IEnumerable<K> keys)
        {
            foreach (var key in keys)
            {
                dict.Remove(key);
            }
        }

        public static void SetTo<K, V>(this IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> items, SetTo setTo = Noggog.SetTo.Whitewash)
        {
            if (setTo == Noggog.SetTo.Whitewash)
            {
                dict.Clear();
                dict.Set(items);
                return;
            }
            var toRemove = new HashSet<K>(dict.Keys);
            switch (setTo)
            {
                case Noggog.SetTo.SkipExisting:
                    foreach (var item in items)
                    {
                        toRemove.Remove(item.Key);
                        if (!dict.ContainsKey(item.Key))
                        {
                            dict.Add(item);
                        }
                    }
                    dict.Remove(toRemove);
                    break;
                case Noggog.SetTo.SetExisting:
                    foreach (var item in items)
                    {
                        toRemove.Remove(item.Key);
                        dict.Add(item);
                    }
                    dict.Remove(toRemove);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static IReadOnlyDictionary<K, V> Empty<K, V>() => DictEmptyExt<K, V>.Empty;

        public static IReadOnlyDictionary<TKey, TTargetValue> Covariant<TKey, TSourceValue, TTargetValue>(this IReadOnlyDictionary<TKey, TSourceValue> dict)
            where TSourceValue : TTargetValue
        {
            return new DictionaryCovariantWrapper<TKey, TSourceValue, TTargetValue>(dict);
        }

        private static class DictEmptyExt<K, V>
        {
            public static Dictionary<K, V> Empty = new Dictionary<K, V>();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
