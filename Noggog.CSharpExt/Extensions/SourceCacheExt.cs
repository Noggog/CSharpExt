using DynamicData;
using System.Diagnostics.CodeAnalysis;

namespace Noggog;

public static class SourceCacheExt
{
    public static bool TryGetValue<TObject, TKey>(this IObservableCache<TObject, TKey> cache, TKey key, [MaybeNullWhen(false)] out TObject value)
        where TKey : notnull
        where TObject : notnull
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
        where TKey : notnull
        where TObject : notnull
    {
        if (!TryGetValue(cache, key, out var val))
        {
            throw new KeyNotFoundException();
        }
        return val;
    }
}