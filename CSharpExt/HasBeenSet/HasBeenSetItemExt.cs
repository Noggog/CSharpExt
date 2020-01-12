﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noggog
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
                not.Set(rhs.Item);
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(def.Item);
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
                if (def != null)
                {
                    not.Set(converter(rhs.Item, def.Item));
                }
                else
                {
                    not.Set(converter(rhs.Item, default(T)));
                }
            }
            else if (def?.HasBeenSet ?? false)
            {
                not.Set(converter(def.Item, default(T)));
            }
            else
            {
                not.Unset();
            }
        }

        public static void SetIfSucceeded<T, R>(this IHasItem<T> not, TryGet<R> tryGet)
            where R : T
        {
            if (tryGet.Succeeded)
            {
                not.Item = tryGet.Value;
            }
        }

        public static void SetIfSucceededOrDefault<T, R>(this IHasItem<T> not, TryGet<R> tryGet)
            where R : T
        {
            if (tryGet.Succeeded)
            {
                not.Item = tryGet.Value;
            }
            else
            {
                not.Unset();
            }
        }
    }
}
