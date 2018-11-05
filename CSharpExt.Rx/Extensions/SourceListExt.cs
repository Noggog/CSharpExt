using CSharpExt.Rx;
using DynamicData;
using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class SourceListExt
    {
        public static void SetTo<T>(this ISourceList<T> list, IEnumerable<T> items)
        {
            list.Edit((l) =>
            {
                l.SetTo(items);
            });
        }

        public static void SetToWithDefault<T>(
            this ISourceSetList<T> not,
            IObservableSetList<T> rhs,
            IObservableSetList<T> def)
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

        public static void SetToWithDefault<V>(
            this ISourceSetList<V> not,
            IObservableSetList<V> rhs,
            IObservableSetList<V> def,
            Func<V, V, V> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Item.Select((t) => converter(t, default(V))));
                }
                else
                {
                    int i = 0;
                    not.SetTo(
                        rhs.Item.Select((t) =>
                        {
                            V defVal = default(V);
                            if (def.Count > i)
                            {
                                defVal = def[i];
                            }
                            return converter(t, defVal);
                        }));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(
                    def.Item.Select((t) => converter(t, default(V))));
            }
            else
            {
                not.Unset();
            }
        }
    }
}
