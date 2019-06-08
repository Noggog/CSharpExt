using CSharpExt.Rx;
using DynamicData;
using Noggog;
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

        public static void SetTo<T>(this ISourceSetList<T> list, IEnumerable<T> items)
        {
            list.Edit((l) =>
            {
                l.SetTo(items);
            });
        }

        public static void SetToWithDefault<T>(
            this ISetList<T> not,
            IReadOnlySetList<T> rhs,
            IReadOnlySetList<T> def)
        {
            if (rhs.HasBeenSet)
            {
                not.SetTo(rhs);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.SetTo(def);
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<T>(
            this IList<T> not,
            IReadOnlyList<T> rhs,
            IReadOnlyList<T> def)
        {
            not.SetTo(rhs);
        }

        public static void SetToWithDefault<V>(
            this ISetList<V> not,
            IReadOnlySetList<V> rhs,
            IReadOnlySetList<V> def,
            Func<V, V, V> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.SetTo(
                        rhs.Select((t) => converter(t, default)));
                }
                else
                {
                    int i = 0;
                    not.SetTo(
                        rhs.Select((t) =>
                        {
                            V defVal = default;
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
                    def.Select((t) => converter(t, default)));
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<V>(
            this IList<V> not,
            IReadOnlyList<V> rhs,
            IReadOnlyList<V> def,
            Func<V, V, V> converter)
        {
            if (def == null)
            {
                not.SetTo(
                    rhs.Select((t) => converter(t, default)));
            }
            else
            {
                int i = 0;
                not.SetTo(
                    rhs.Select((t) =>
                    {
                        V defVal = default;
                        if (def.Count > i)
                        {
                            defVal = def[i];
                        }
                        return converter(t, defVal);
                    }));
            }
        }
    }
}
