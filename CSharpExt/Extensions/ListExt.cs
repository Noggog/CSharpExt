using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Noggog
{
    public static class ListExt
    {
        public static int BinarySearch<T>(this IReadOnlyList<T> list, T value)
        {
            if (list.Count == 0) return ~0;
            var comp = Comparer<T>.Default;
            int low = 0;
            int high = list.Count - 1;
            while (low < high)
            {
                var index = low + (high - low) / 2;
                var result = comp.Compare(list[index], value);
                if (result == 0)
                {
                    return index;
                }
                else if (result < 0)
                {
                    low = index + 1;
                }
                else
                {
                    high = index - 1;
                }
            }
            var c = comp.Compare(list[low], value);
            if (c < 0)
            {
                low++;
            }
            else if (c == 0)
            {
                return low;
            }
            return ~low;
        }

        // IList does not implement IReadOnlyList
        public static int BinarySearch<T>(this IList<T> list, T value)
        {
            if (list.Count == 0) return ~0;
            var comp = Comparer<T>.Default;
            int low = 0;
            int high = list.Count - 1;
            while (low < high)
            {
                var index = low + (high - low) / 2;
                var result = comp.Compare(list[index], value);
                if (result == 0)
                {
                    return index;
                }
                else if (result < 0)
                {
                    low = index + 1;
                }
                else
                {
                    high = index - 1;
                }
            }
            var c = comp.Compare(list[low], value);
            if (c < 0)
            {
                low++;
            }
            else if (c == 0)
            {
                return low;
            }
            return ~low;
        }

        // To avoid compiler confusion
        public static int BinarySearch<T>(this List<T> list, T value)
        {
            return BinarySearch<T>((IReadOnlyList<T>)list, value);
        }

        public static bool InRange<T>(this IReadOnlyList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        public static bool TryGet<T>(this IReadOnlyList<T> list, int index, out T item)
        {
            if (!InRange(list, index))
            {
                item = default;
                return false;
            }
            item = list[index];
            return true;
        }

        public static T TryGet<T>(this IReadOnlyList<T> list, int index)
        {
            if (!InRange(list, index))
            {
                return default(T);
            }
            return list[index];
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

        public static void SetTo<T>(this IList<T> list, IEnumerable<T> items, bool checkEquality = false)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (i >= list.Count)
                {
                    list.Add(item);
                }
                else if (checkEquality)
                {
                    if (!EqualityComparer<T>.Default.Equals(list[i], item))
                    {
                        list[i] = item;
                    }
                }
                else
                {
                    list[i] = item;
                }
                i++;
            }

            list.RemoveToCount(i);
        }

        public static void SetTo<T, R>(this IList<T> list, IEnumerable<R> items, Func<R, T> converter)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (i >= list.Count)
                {
                    list.Add(converter(item));
                }
                else
                {
                    list[i] = converter(item);
                }
                i++;
            }

            list.RemoveToCount(i);
        }

        public static void SetTo<T>(this ISetList<T> list, IEnumerable<T> items)
        {
            ListExt.SetTo((IList<T>)list, items);
            list.HasBeenSet = true;
        }

        public static void SetTo<T, R>(this ISetList<T> list, IEnumerable<R> items, Func<R, T> converter)
        {
            ListExt.SetTo((IList<T>)list, items, converter);
            list.HasBeenSet = true;
        }

        public static void SetTo<T>(this IList<T> list, T item)
        {
            list.Clear();
            list.Add(item);
        }

        public static void RemoveToCount<T>(this IList<T> list, int count)
        {
            var toRemove = list.Count - count;
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

        public static T LastByIndex<T>(this IReadOnlyList<T> list)
        {
            return list[list.Count - 1];
        }

        public static IEnumerable<T> For<T>(this IList<T> list, int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                yield return list[i];
            }
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
            if (def != null) throw new NotImplementedException();
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

        public static IReadOnlyList<T> Empty<T>() => ListEmptyExt<T>.Empty;

        private static class ListEmptyExt<T>
        {
            public static List<T> Empty = new List<T>();
        }
    }
}
