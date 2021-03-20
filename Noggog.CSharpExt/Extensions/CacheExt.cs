using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public static void SetTo<V, K>(this ICache<V, K> cache, Func<V, K> keySelector, IEnumerable<V> items, SetTo setTo = Noggog.SetTo.Whitewash)
        {
            if (setTo == Noggog.SetTo.Whitewash)
            {
                SetTo(cache, items);
                return;
            }
            var toRemove = new HashSet<K>(cache.Keys);
            var keyPairs = items.Select(i => new KeyValuePair<K, V>(keySelector(i), i)).ToArray();
            toRemove.Remove(keyPairs.Select(kv => kv.Key));
            cache.Remove(toRemove);
            switch (setTo)
            {
                case Noggog.SetTo.SkipExisting:
                    foreach (var item in keyPairs)
                    {
                        if (!cache.ContainsKey(item.Key))
                        {
                            cache.Set(item.Value);
                        }
                    }
                    break;
                case Noggog.SetTo.SetExisting:
                    cache.Set(items);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetTo<V, K>(this ISourceCache<V, K> cache, IEnumerable<V> items)
            where K : notnull
        {
            cache.Clear();
            cache.AddOrUpdate(items);
        }

        public static void SetTo<V, K>(this ISourceCache<V, K> cache, Func<V, K> keySelector, IEnumerable<V> items, SetTo setTo = Noggog.SetTo.Whitewash)
            where K : notnull
        {
            if (setTo == Noggog.SetTo.Whitewash)
            {
                SetTo(cache, items);
                return;
            }
            var toRemove = new HashSet<K>(cache.Keys);
            var keyPairs = items.Select(i => new KeyValuePair<K, V>(keySelector(i), i)).ToArray();
            toRemove.Remove(keyPairs.Select(kv => kv.Key));
            cache.Remove(toRemove);
            switch (setTo)
            {
                case Noggog.SetTo.SkipExisting:
                    foreach (var item in keyPairs)
                    {
                        if (!cache.Lookup(item.Key).HasValue)
                        {
                            cache.AddOrUpdate(item.Value);
                        }
                    }
                    break;
                case Noggog.SetTo.SetExisting:
                    cache.AddOrUpdate(items);
                    break;
                default:
                    throw new NotImplementedException();
            }
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

        public static bool TryGetValue<TObject, TKey>(this IReadOnlyCache<TObject, TKey> cache, TKey key, [MaybeNullWhen(false)] out TObject value)
        {
            // ToDo
            // Improve to not double query
            if (cache.ContainsKey(key))
            {
                value = cache[key];
                return true;
            }
            value = default;
            return false;
        }
    }
}
