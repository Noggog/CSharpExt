using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void SetTo<V, K>(this ICache<V, K> cache, IEnumerable<V> items)
        {
            cache.Clear();
            cache.Set(items);
        }

        public static TObject SetReturn<TObject, TKey>(this ICache<TObject, TKey> source, TObject item)
        {
            source.Set(item);
            return item;
        }

        public static void SetToWithDefault<TItem, TRhs, TKey>(
            this ICache<TItem, TKey> cache,
            IReadOnlyDictionary<TKey, TRhs> rhs,
            IReadOnlyDictionary<TKey, TRhs>? def,
            Func<TRhs, TRhs?, TItem> converter)
            where TRhs : class
        {
            if (def == null)
            {
                cache.SetTo(
                    rhs.Values.Select((t) => converter(t, default)));
            }
            else
            {
                cache.SetTo(
                    rhs.Select((t) =>
                    {
                        TRhs? defVal;
                        if (def.TryGetValue(t.Key, out var get))
                        {
                            defVal = get;
                        }
                        else
                        {
                            defVal = default;
                        }
                        return converter(t.Value, defVal);
                    }));
            }
        }

        public static void SetToWithDefault<TItem, TRhs, TKey>(
            this ICache<TItem, TKey> not,
            IReadOnlyCache<TRhs, TKey> rhs,
            IReadOnlyCache<TRhs, TKey>? def,
            Func<TRhs, TRhs?, TItem> converter)
            where TRhs : class
        {
            if (def == null)
            {
                not.SetTo(
                    rhs.Items.Select((t) => converter(t, default)));
            }
            else
            {
                not.SetTo(
                    rhs.Select((t) =>
                    {
                        TRhs? defVal;
                        if (def.TryGetValue(t.Key, out var get))
                        {
                            defVal = get;
                        }
                        else
                        {
                            defVal = default;
                        }
                        return converter(t.Value, defVal);
                    }));
            }
        }

        public static void SetToWithDefault<TItem, TKey>(
            this ICache<TItem, TKey> not,
            ICache<TItem, TKey> rhs,
            ICache<TItem, TKey>? def,
            Func<TItem, TItem?, TItem> converter)
            where TItem : class
        {
            if (def == null)
            {
                not.SetTo(
                    rhs.Items.Select((t) => converter(t, default)));
            }
            else
            {
                not.SetTo(
                    rhs.Select((t) =>
                    {
                        TItem? defVal;
                        if (def.TryGetValue(t.Key, out var get))
                        {
                            defVal = get;
                        }
                        else
                        {
                            defVal = default;
                        }
                        return converter(t.Value, defVal);
                    }));
            }
        }
    }
}
