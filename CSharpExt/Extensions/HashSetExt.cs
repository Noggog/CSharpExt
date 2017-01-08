using System;
using System.Collections.Generic;

namespace System
{
    public static class HashSetExt
    {
        public static T[] ToArray<T>(this HashSet<T> set)
        {
            T[] arr = new T[set.Count];
            int i = 0;
            foreach (T element in set)
            {
                arr[i] = element;
                i++;
            }
            return arr;
        }

        public static void Add<T>(this HashSet<T> set, IEnumerable<T> enumer)
        {
            foreach (var e in enumer)
            {
                set.Add(e);
            }
        }

        public static void Add<T>(this HashSet<T> set, params T[] objs)
        {
            foreach (var e in objs)
            {
                set.Add(e);
            }
        }
    }
}
