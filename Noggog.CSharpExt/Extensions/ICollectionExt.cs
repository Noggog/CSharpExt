using System;
using System.Collections.Generic;

namespace Noggog
{
    public static class ICollectionExt
    {
        public static void SetTo<T>(ICollection<T> coll, params T[] items)
        {
            SetTo(coll, ((IEnumerable<T>)items));
        }

        public static void SetTo<T>(ICollection<T> coll, IEnumerable<T> en)
        {
            coll.Clear();
            foreach (var e in en)
            {
                coll.Add(e);
            }
        }

        public static void Last<T>(
            this ICollection<T> coll,
            Action<T> each,
            Action<T> last)
        {
            int i = 0;
            foreach (var item in coll)
            {
                if (i == coll.Count - 1)
                {
                    last(item);
                }
                else
                {
                    each(item);
                }
                i++;
            }
        }

        public static void Last<T>(
            this ICollection<T> coll,
            Action<T, bool> each)
        {
            int i = 0;
            foreach (var item in coll)
            {
                each(item, i == coll.Count - 1);
                i++;
            }
        }

        public static IEnumerable<(T Item, bool Last)> IterateMarkLast<T>(
            this ICollection<T> coll)
        {
            int count = coll.Count;
            foreach (var item in coll)
            {
                yield return (item, --count == 0);
            }
        }

        public static void First<T>(
            this ICollection<T> coll,
            Action<T, bool> each)
        {
            bool first = true;
            foreach (var item in coll)
            {
                each(item, first);
                first = false;
            }
        }

        public static ICollectionGetter<T> ToGetter<T>(this ICollection<T> coll)
        {
            return new CollectionGetterWrapper<T>(coll);
        }
    }
}
