using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Noggog
{
    public static class DictionaryExt
    {
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            foreach (var item in items)
            {
                dict[item.Key] = item.Value;
            }
        }

        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
            where TValue : new()
        {
            if (!dict.TryGetValue(key, out var ret))
            {
                ret = new TValue();
                dict[key] = ret;
            }
            return ret;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> getNew)
        {
            if (!dict.TryGetValue(key, out var ret))
            {
                ret = getNew();
                dict[key] = ret;
            }
            return ret;
        }

        public static TValue UpdateOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue?, TValue> getNew)
        {
            if (dict.TryGetValue(key, out var ret))
            {
                ret = getNew(ret);
                dict[key] = ret;
            }
            else
            {
                ret = getNew(default);
                dict[key] = ret;
            }
            return ret;
        }

        public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            if (!dict.TryGetValue(key, out var ret))
            {
                return default;
            }
            return ret;
        }

        public static IEnumerable<KeyValuePair<TKey, TRet>> SelectAgainst<TKey, TValue, TRet>(
            this IReadOnlyDictionary<TKey, TValue> lhs, 
            IReadOnlyDictionary<TKey, TValue> rhs, 
            Func<TKey, TValue, TValue, TRet> selector, 
            out bool equal)
        {
            List<KeyValuePair<TKey, TRet>> ret = new List<KeyValuePair<TKey, TRet>>();
            equal = lhs.Count == rhs.Count;
            foreach (var item in lhs)
            {
                if (!rhs.TryGetValue(item.Key, out var rhsItem))
                {
                    equal = false;
                    continue;
                }
                ret.Add(
                    new KeyValuePair<TKey, TRet>(
                        item.Key,
                        selector(item.Key, item.Value, rhsItem)));
            }
            return ret;
        }

        public static void Remove<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
            {
                dict.Remove(key);
            }
        }

        public static void SetTo<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> items, SetTo setTo = Noggog.SetTo.Whitewash)
        {
            if (setTo == Noggog.SetTo.Whitewash)
            {
                dict.Clear();
                dict.Set(items);
                return;
            }
            var toRemove = new HashSet<TKey>(dict.Keys);
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

        public static IReadOnlyDictionary<TKey, TValue> Empty<TKey, TValue>()
            where TKey : notnull
        {
            return DictEmptyExt<TKey, TValue>.Empty;
        }

#if NETSTANDARD2_0
#else
        public static IReadOnlyDictionary<TKey, TTargetValue> Covariant<TKey, TSourceValue, TTargetValue>(this IReadOnlyDictionary<TKey, TSourceValue> dict)
            where TSourceValue : TTargetValue
        {
            return new DictionaryCovariantWrapper<TKey, TSourceValue, TTargetValue>(dict);
        }
#endif

        private static class DictEmptyExt<TKey, TValue>
            where TKey : notnull
        {
            public static Dictionary<TKey, TValue> Empty = new Dictionary<TKey, TValue>();
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> keyValuePairs)
            where TKey : notnull
        {
            return keyValuePairs.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
