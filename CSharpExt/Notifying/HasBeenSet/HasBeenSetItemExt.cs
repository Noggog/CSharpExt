using Noggog.Notifying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class HasBeenSetItemExt
    {
        public static void SetToWithDefault<T>(
            this IHasBeenSetItem<T> not,
            IHasBeenSetItemGetter<T> rhs,
            IHasBeenSetItemGetter<T> def)
        {
            if (rhs.HasBeenSet)
            {
                not.Set(rhs.Value);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(def.Value);
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetToWithDefault<T>(
            this IHasBeenSetItem<T> not,
            IHasBeenSetItemGetter<T> rhs,
            IHasBeenSetItemGetter<T> def,
            Func<T, T, T> converter)
        {
            if (rhs.HasBeenSet)
            {
                if (def == null)
                {
                    not.Set(converter(rhs.Value, def.Value));
                }
                else
                {
                    not.Set(converter(rhs.Value, default(T)));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(converter(def.Value, default(T)));
            }
            else
            {
                not.Unset();
            }
        }
    }
}
