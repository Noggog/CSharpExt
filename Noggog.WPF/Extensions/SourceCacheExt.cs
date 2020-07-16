using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Noggog.WPF
{
    public static class SourceCacheExt
    {
        public static bool TryGetValue<TObject, TKey>(this IObservableCache<TObject, TKey> cache, TKey key, [MaybeNullWhen(false)] out TObject value)
        {
            var lookup = cache.Lookup(key);
            if (lookup.HasValue)
            {
                value = lookup.Value;
                return true;
            }
            value = default;
            return false;
        }

        public static TObject Get<TObject, TKey>(this IObservableCache<TObject, TKey> cache, TKey key)
        {
            if (!TryGetValue(cache, key, out var val))
            {
                throw new KeyNotFoundException();
            }
            return val;
        }
    }
}
