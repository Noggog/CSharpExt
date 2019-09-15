using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class ObservableCacheExt
    {
        public static bool TryGetValue<V, K>(this IObservableCache<V, K> cache, K key, out V value)
        {
            var opt = cache.Lookup(key);
            if (opt.HasValue)
            {
                value = opt.Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
