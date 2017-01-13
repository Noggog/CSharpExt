using Noggog;
using System;
using System.Collections.Generic;

namespace System
{
    public static class ListExt
    {
        public static int BinarySearch<T>(this IList<T> list, T value)
        {
            if (list.Count == 0) return -1;
            var comp = Comparer<T>.Default;
            int low = 0;
            int high = list.Count - 1;
            while (low < high)
            {
                var m = low + (high - low) / 2;
                if (comp.Compare(list[m], value) < 0)
                {
                    low = m + 1;
                }
                else
                {
                    high = m - 1;
                }
            }
            if (comp.Compare(list[low], value) < 0)
            {
                low++;
            }
            return low;
        }

        public static bool InRange<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static List<T> Populate<T>(this List<T> list, int num) where T : new()
        {
            for (int i = 0; i < num; i++)
            {
                list.Add(new T());
            }
            return list;
        }

        public static T Take<T>(this List<T> list)
        {
            T item = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return item;
        }

        public static T RandomTake<T>(this List<T> list, RandomSource rand)
        {
            T item;
            RandomTake(list, rand, out item);
            return item;
        }

        public static bool RandomTake<T>(this List<T> list, RandomSource rand, out T item)
        {
            if (list.Count == 0)
            {
                item = default(T);
                return false;
            }
            int r = rand.Next(list.Count);
            item = list[r];
            list.RemoveAt(r);
            return true;
        }

        public static bool Random<T>(this List<T> list, RandomSource rand, out T item)
        {
            if (list.Count == 0)
            {
                item = default(T);
                return false;
            }
            item = list[rand.Next(list.Count)];
            return true;
        }

        public static T Random<T>(this List<T> list, RandomSource rand)
        {
            if (list.Count > 0)
            {
                return list[rand.Next(list.Count)];
            }
            return default(T);
        }

        public static IEnumerable<T> Random<T>(this List<T> list, RandomSource rand, int amount)
        {
            foreach (T t in list.Randomize(rand))
            {
                if (amount <= 0)
                {
                    break;
                }
                yield return t;
                amount--;
            }
        }

        public static bool TryRemoveAt<T>(this List<T> list, int index)
        {
            if (list.Count > index)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void SetTo<T>(this IList<T> list, IEnumerable<T> items)
        {
            list.Clear();
            list.AddRange(items);
        }

        public static void SetTo<T>(this IList<T> list, T item)
        {
            list.Clear();
            list.Add(item);
        }

        public static void RemoveEnd<T>(this IList<T> list, int fromIndex)
        {
            var toRemove = list.Count - fromIndex;
            for (; toRemove > 0; toRemove--)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

        public static void RemoveAt<T>(this IList<T> list, int index, out T item)
        {
            item = list[index];
            list.RemoveAt(index);
        }

        public static bool Remove<T>(this IList<T> list, T item, out int index)
        {
            EqualityComparer<T> comp = EqualityComparer<T>.Default;

            for (int i = 0; i < list.Count; i++)
            {
                if (comp.Equals(list[i], item))
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }
    }
}
