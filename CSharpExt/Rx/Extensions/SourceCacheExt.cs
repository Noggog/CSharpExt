﻿using CSharpExt.Rx;
using DynamicData;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
{
    public static class SourceCacheExt
    {
        public static void SetTo<V, K>(this ISourceCache<V, K> list, IEnumerable<V> items)
        {
            list.Edit((l) =>
            {
                l.Load(items);
            });
        }

        public static void SetToWithDefault<V, K>(
            this ISourceSetCache<V, K> not,
            IHasBeenSetItemGetter<IEnumerable<V>> rhs,
            IHasBeenSetItemGetter<IEnumerable<V>> def)
        {
            if (rhs.HasBeenSet)
            {
                not.SetTo(rhs.Item);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(def.Item);
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<V, K>(
            this ISourceSetCache<V, K> not,
            IObservableSetCache<V, K> rhs,
            IObservableSetCache<V, K> def,
            Func<V, V, V> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t, default)));
                }
                else
                {
                    not.SetTo(
                        rhs.KeyValues.Select((t) =>
                        {
                            if (!def.TryGetValue(t.Key, out var defVal))
                            {
                                defVal = default;
                            }
                            return converter(t.Value, defVal);
                        }));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Item.Select((t) => converter(t, default)));
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<V, K>(
            this ISourceCache<V, K> not,
            IReadOnlyDictionary<K, V> rhs,
            IReadOnlyDictionary<K, V> def,
            Func<V, V, V> converter)
        {
            if (def == null)
            {
                not.SetTo(
                    rhs.Values.Select((t) => converter(t, default)));
            }
            else
            {
                not.SetTo(
                    rhs.Select((t) =>
                    {
                        if (!def.TryGetValue(t.Key, out var defVal))
                        {
                            defVal = default;
                        }
                        return converter(t.Value, defVal);
                    }));
            }
        }

        public static TObject SetReturn<TObject, TKey>(this ISourceCache<TObject, TKey> source, TObject item)
        {
            ObservableCacheEx.Set(source, item);
            source.Set(item);
            return item;
        }
    }
}
