using Noggog;
using System;
using System.Collections.Generic;

namespace System
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

        public static V TryCreateValue<K, V>(this Dictionary<K, V> dict, K key)
            where V : new()
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = new V();
                dict[key] = ret;
            }
            return ret;
        }

        public static V TryCreateValue<K, V>(this Dictionary<K, V> dict, K key, Func<V> getNew)
        {
            if (!dict.TryGetValue(key, out V ret))
            {
                ret = getNew();
                dict[key] = ret;
            }
            return ret;
        }

        public static bool AddRemove<K, V>(this Dictionary<K, V> dict, K key, V value, AddRemove addRem)
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

        public static bool Modify<K, V>(this Dictionary<K, V> dict, K key, V value, AddRemove addRem)
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

        public static bool Modify<K, V>(this Dictionary<K, V> dict, K key, V value, AddRemoveModify addRem)
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
    }
}