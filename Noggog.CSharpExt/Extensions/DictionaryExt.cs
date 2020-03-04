using Noggog;
using System;
using System.Collections.Generic;

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

        public static V TryCreateValue<K, V>(this IDictionary<K, V> dict, K key)
            where V : new()
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = new V();
                dict[key] = ret;
            }
            return ret;
        }

        public static V TryCreateValue<K, V>(this IDictionary<K, V> dict, K key, Func<V> getNew)
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = getNew();
                dict[key] = ret;
            }
            return ret;
        }

        public static bool AddRemove<K, V>(this IDictionary<K, V> dict, K key, V value, AddRemove addRem)
        {
            switch (addRem)
            {
                case Noggog.AddRemove.Add:
                    dict.Add(key, value);
                    break;
                case Noggog.AddRemove.Remove:
                    return dict.Remove(key);
                default:
                    throw new NotImplementedException();
            }
            return false;
        }

        public static bool Modify<K, V>(this IDictionary<K, V> dict, K key, V value, AddRemove addRem)
        {
            switch (addRem)
            {
                case Noggog.AddRemove.Add:
                    dict[key] = value;
                    break;
                case Noggog.AddRemove.Remove:
                    return dict.Remove(key);
                default:
                    throw new NotImplementedException();
            }
            return false;
        }

        public static bool Modify<K, V>(this IDictionary<K, V> dict, K key, V value, AddRemoveModify addRem)
        {
            switch (addRem)
            {
                case Noggog.AddRemoveModify.Add:
                case Noggog.AddRemoveModify.Modify:
                    dict[key] = value;
                    break;
                case Noggog.AddRemoveModify.Remove:
                    return dict.Remove(key);
                default:
                    throw new NotImplementedException();
            }
            return false;
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

        public static void SetTo<K, V>(this IDictionary<K, V> dict, IEnumerable<KeyValuePair<K, V>> items)
        {
            dict.Clear();
            dict.Set(items);
        }
    }
}