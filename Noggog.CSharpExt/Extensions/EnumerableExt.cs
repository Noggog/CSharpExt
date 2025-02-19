using System.Collections;

namespace Noggog;

public static class EnumerableExt
{
    public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
    {
        return typeof(T);
    }

    public static IEnumerable<T> AsEnumerable<T>(this T item)
    {
        yield return item;
    }

    public static bool Any(this IEnumerable enumer)
    {
        foreach (var _ in enumer)
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

    public static IEnumerable<LastMarkedItem<T>> IterateMarkLast<T>(this IEnumerable<T> en)
    {
        T last = default(T)!;
        bool first = true;
        foreach (var item in en)
        {
            if (!first)
            {
                yield return new(last, false);
            }
            else
            {
                first = false;
            }
            last = item;
        }
        if (!first)
        {
            yield return new(last, true);
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

    public static IEnumerable<IndexedItem<T>> WithIndex<T>(this IEnumerable<T> en)
    {
        int index = 0;
        foreach (var item in en)
        {
            yield return new (index++, item);
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

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> e)
        where T : class => e.Where(i => i != null)!;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> e)
        where T : struct => e.Where(i => i.HasValue).Select(i => i!.Value);

    // Keep NotNull variants, as ReactiveUI has a collision with WhereNotNull
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> e)
        where T : class
    {
        return e.Where(i => i != null)!;
    }

    // Keep NotNull variants, as ReactiveUI has a collision with WhereNotNull
    public static IEnumerable<T> NotNull<T>(this IEnumerable<T?> e)
        where T : struct
    {
        return e.Where(i => i.HasValue)
            .Select(i => i!.Value);
    }

    public static ExtendedList<T> ToExtendedList<T>(this IEnumerable<T> e)
    {
        return new ExtendedList<T>(e);
    }

    public static IEnumerable<T> Catch<T>(this IEnumerable<T> e, Action<Exception> onException)
    {
        using (var enumerator = e.GetEnumerator())
        {
            bool next = true;

            while (next)
            {
                try
                {
                    next = enumerator.MoveNext();
                }
                catch (Exception ex)
                {
                    onException(ex);
                    continue;
                }

                if (next)
                {
                    yield return enumerator.Current;
                }
            }
        }
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T>? e)
    {
        if (e == null) return [];
        return e;
    }

    public static bool SequenceEqualNullable<T>(this IEnumerable<T>? e1, IEnumerable<T>? e2)
    {
        if (e1 == null && e2 == null) return true;
        if (e1 == null || e2 == null) return false;
        return e1.SequenceEqual(e2);
    }
 
    public static bool SequenceEqualToItems<T>(this IEnumerable<T> e, params T[] rhs) 
    { 
        return e.SequenceEqual((IEnumerable<T>)rhs); 
    }
 
    public static bool SequenceEqual<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, T, bool> equality) 
    { 
        if (ReferenceEquals(lhs, rhs)) return true;
        return lhs
            .Zip(rhs, equality)
            .All(x => x);
    }

    public static IEnumerable<T> StartWith<T>(this IEnumerable<T> e, T item)
    {
        yield return item;
        foreach (var followupItem in e)
        {
            yield return followupItem;
        }
    }

    public static IEnumerable<T> StartWith<T>(this IEnumerable<T> e, T item, params T[] items)
    {
        yield return item;
        foreach (var followupItem in items)
        {
            yield return followupItem;
        }
        foreach (var followupItem in e)
        {
            yield return followupItem;
        }
    }

    public static IEnumerable<T> StartWith<T>(this IEnumerable<T> e, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            yield return item;
        }
        foreach (var followupItem in e)
        {
            yield return followupItem;
        }
    }

    public static int Max(this IEnumerable<int> e, int seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static uint Max(this IEnumerable<uint> e, uint seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static short Max(this IEnumerable<short> e, short seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static ushort Max(this IEnumerable<ushort> e, ushort seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static long Max(this IEnumerable<long> e, long seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static ulong Max(this IEnumerable<ulong> e, ulong seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static byte Max(this IEnumerable<byte> e, byte seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static sbyte Max(this IEnumerable<sbyte> e, sbyte seedValue)
    {
        return e.StartWith(seedValue).Max();
    }

    public static IEnumerable<T> Take<T>(this IEnumerable<T> e, int? amount)
    {
        if (amount == null) return e;
        return System.Linq.Enumerable.Take(e, amount.Value);
    }
}

public static class EnumerableExt<T>
{
    public static readonly IEnumerable<T> Empty = Array.Empty<T>();
}