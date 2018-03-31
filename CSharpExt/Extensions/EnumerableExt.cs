using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
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

        public static IEnumerable<T> And<T>(this IEnumerable<T> enumer2, T item)
        {
            foreach (var e in enumer2)
            {
                yield return e;
            }
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
    }

    public static class EnumerableExt<T>
    {
        public static readonly IEnumerable<T> EMPTY = new T[0];
    }
}
