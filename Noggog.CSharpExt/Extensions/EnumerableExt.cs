using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Noggog
{
    public static class EnumerableExt
    {
        public static Type GetEnumeratedType<T>(this IEnumerable<T> e)
        {
            return typeof(T);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> rhs, Func<T, bool> filter)
        {
            foreach (T t in rhs)
            {
                if (filter(t))
                {
                    yield return t;
                }
            }
        }

        public static IEnumerable<T> Single<T>(this T item)
        {
            yield return item;
        }

        public static bool Any(this IEnumerable enumer)
        {
            foreach (var item in enumer)
            {
                return true;
            }
            return false;
        }

        public static bool CountGreaterThan(this IEnumerable enumer, uint count)
        {
            foreach (var e in enumer)
            {
                if (count-- <= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> enumer, IEnumerable<T> enumer2)
        {
            foreach (var e in enumer)
            {
                yield return e;
            }
            foreach (var e in enumer2)
            {
                yield return e;
            }
        }

        public static IEnumerable<T> And<T>(this T item, IEnumerable<T> enumer2)
        {
            yield return item;
            if (enumer2 == null) yield break;
            foreach (var e in enumer2)
            {
                yield return e;
            }
        }

        public static IEnumerable<T> AndSingle<T>(this T item1, T item2)
        {
            yield return item1;
            yield return item2;
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> enumer2, T item)
        {
            foreach (var e in enumer2)
            {
                yield return e;
            }
            yield return item;
        }

        public static IEnumerable<T> AndWhen<T>(this IEnumerable<T> enumer, IEnumerable<T> enumer2, Func<bool> when)
        {
            foreach (var e in enumer)
            {
                yield return e;
            }
            if (!when()) yield break;
            foreach (var e in enumer2)
            {
                yield return e;
            }
        }

        public static IEnumerable<T> AndWhen<T>(this T item, IEnumerable<T> enumer2, Func<bool> when)
        {
            yield return item;
            if (enumer2 == null) yield break;
            if (!when()) yield break;
            foreach (var e in enumer2)
            {
                yield return e;
            }
        }

        public static IEnumerable<T> AndWhenSingle<T>(this T item, T item2, Func<bool> when)
        {
            yield return item;
            if (!when()) yield break;
            yield return item2;
        }

        public static IEnumerable<T> AndWhen<T>(this IEnumerable<T> enumer2, T item, Func<bool> when)
        {
            foreach (var e in enumer2)
            {
                yield return e;
            }
            if (!when()) yield break;
            yield return item;
        }

        public static IEnumerable<R> SelectWhere<T, R>(this IEnumerable<T> enumer, Func<T, TryGet<R>> conv)
        {
            foreach (var item in enumer)
            {
                var get = conv(item);
                if (get.Succeeded)
                {
                    yield return get.Value;
                }
            }
        }

        public delegate bool SelectWhereSelector<T, R>(T item, out R returnItem);
        public static IEnumerable<R> SelectWhere<T, R>(this IEnumerable<T> enumer, SelectWhereSelector<T, R> conv)
        {
            foreach (var item in enumer)
            {
                if (conv(item, out var ret))
                {
                    yield return ret;
                }
            }
        }

        public static IEnumerable<T> Randomize<T>(this IEnumerable<T> e, RandomSource rand)
        {
            return e.OrderBy<T, int>((item) => rand.Next());
        }

        public static IEnumerable<R> SelectAgainst<T, R>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, T, R> selector, out bool countEqual)
        {
            List<R> ret = new List<R>();
            var lhsEnumer = lhs.GetEnumerator();
            var rhsEnumer = rhs.GetEnumerator();
            while (lhsEnumer.MoveNext())
            {
                if (!rhsEnumer.MoveNext())
                {
                    countEqual = false;
                    return ret;
                }
                ret.Add(selector(lhsEnumer.Current, rhsEnumer.Current));
            }
            countEqual = !rhsEnumer.MoveNext();
            return ret;
        }

        public static IEnumerable<R> SelectAgainst<T, R>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, T, R> selector)
        {
            var lhsEnumer = lhs.GetEnumerator();
            var rhsEnumer = rhs.GetEnumerator();
            while (lhsEnumer.MoveNext())
            {
                if (!rhsEnumer.MoveNext()) yield break;
                yield return selector(lhsEnumer.Current, rhsEnumer.Current);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> e, Action<T> toDo)
        {
            foreach (var item in e)
            {
                toDo(item);
            }
        }

        public static IEnumerable<T> First<T>(this IEnumerable<T> en, int amount)
        {
            foreach (var item in en)
            {
                if (amount-- == 0) yield break;
                yield return item;
            }
        }

        public static IEnumerable<(T Item, bool Last)> IterateMarkLast<T>(this IEnumerable<T> en)
        {
            T last = default(T);
            bool first = true;
            foreach (var item in en)
            {
                if (!first)
                {
                    yield return (last, false);
                }
                else
                {
                    first = false;
                }
                last = item;
            }
            if (!first)
            {
                yield return (last, true);
            }
        }

        public static IEnumerable<IEnumerable<T>> Cut<T>(this IEnumerable<T> en, int amount)
        {
            // The list to return.
            List<T> list = new List<T>(amount);

            // Cycle through all of the items.
            foreach (T item in en)
            {
                // Add the item.
                list.Add(item);

                // If the list has the number of elements, return that.
                if (list.Count == amount)
                {
                    // Return the list.
                    yield return list;

                    // Set the list to a new list.
                    list = new List<T>(amount);
                }
            }

            // Return the remainder if there is any,
            if (list.Count != 0)
            {
                // Return the list.
                yield return list;
            }
        }

        public static IEnumerable<R> WhereCastable<T, R>(this IEnumerable<T> en)
        {
            foreach (var item in en)
            {
                if (item is R rhs)
                {
                    yield return rhs;
                }
            }
        }

        public static IEnumerable<(int Index, T Item)> WithIndex<T>(this IEnumerable<T> en)
        {
            int index = 0;
            foreach (var item in en)
            {
                yield return (index++, item);
            }
        }

        public static IEnumerable<T> Distinct<T, R>(this IEnumerable<T> en, Func<T, R> distinctionSelector)
        {
            HashSet<R> existing = new HashSet<R>();
            foreach (var item in en)
            {
                if (existing.Add(distinctionSelector(item)))
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<int> For(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                yield return i;
            }
        }

        public static IEnumerable<int> For(int start, int end, int iterateAmount)
        {
            for (int i = start; i < end; i += iterateAmount)
            {
                yield return i;
            }
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> e)
            where T : class
        {
            return e.Where(i => i != null)
                .Select(i => i!);
        }

        public static ExtendedList<T> ToExtendedList<T>(this IEnumerable<T> e)
        {
            return new ExtendedList<T>(e);
        }

        public static IEnumerable<T> TryIterate<T>(this IEnumerable<T>? e)
        {
            if (e == null) return Enumerable.Empty<T>();
            return e;
        }
    }

    public static class EnumerableExt<T>
    {
        public static readonly IEnumerable<T> Empty = new T[0];
    }
}
