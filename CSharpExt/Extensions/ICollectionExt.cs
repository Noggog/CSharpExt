using System;
using System.Collections.Generic;

namespace System
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
    }
}
