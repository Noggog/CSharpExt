using System.Reactive;
using System.Reactive.Linq;
using DynamicData;

namespace Noggog;

public static class ChangeSetExt
{
    class Comparer<TSource, TRhs> : IComparer<TSource>
    {
        private readonly Func<TSource, TRhs> _selector;
        private readonly IComparer<TRhs> _comp;

        public Comparer(Func<TSource, TRhs> selector, IComparer<TRhs> comp)
        {
            _selector = selector;
            _comp = comp;
        }

        public int Compare(TSource? x, TSource? y)
        {
            if (x == null || y == null) throw new NullReferenceException();
            return _comp.Compare(_selector(x), _selector(y));
        }
    }
        
    public static IObservable<IChangeSet<TObj, TKey>> Sort<TObj, TRhs, TKey>(
        this IObservable<IChangeSet<TObj, TKey>> obs,
        Func<TObj, TRhs> selector,
        IComparer<TRhs> comparer,
        SortOptimisations sortOptimisations = SortOptimisations.None, 
        int resetThreshold = 100,
        IObservable<Unit>? resorter = null)
        where TObj : notnull
        where TKey : notnull
        where TRhs : notnull
    {
        if (resorter == null)
        {
            return obs.Sort(new Comparer<TObj, TRhs>(selector, comparer), sortOptimisations, resetThreshold);
        }
        else
        {
            return obs.Sort(new Comparer<TObj, TRhs>(selector, comparer), resorter, sortOptimisations, resetThreshold);
        }
    }
        
    public static IObservable<IChangeSet<TObj, TKey>> Sort<TObj, TRhs, TKey>(
        this IObservable<IChangeSet<TObj, TKey>> obs,
        Func<TObj, TRhs> selector,
        IObservable<IComparer<TRhs>> comparer,
        SortOptimisations sortOptimisations = SortOptimisations.None, 
        int resetThreshold = 100,
        IObservable<Unit>? resorter = null)
        where TObj : notnull
        where TKey : notnull
        where TRhs : notnull
    {
        if (resorter == null)
        {
            return obs.Sort(comparer.Select(x => new Comparer<TObj, TRhs>(selector, x)), sortOptimisations, resetThreshold);
        }
        else
        {
            return obs.Sort(comparer.Select(x => new Comparer<TObj, TRhs>(selector, x)), resorter, sortOptimisations, resetThreshold);
        }
    }
        
    public static IObservable<IChangeSet<TObj>> Sort<TObj, TRhs>(
        this IObservable<IChangeSet<TObj>> obs,
        Func<TObj, TRhs> selector,
        IComparer<TRhs> comparer,
        SortOptions options = SortOptions.None,
        IObservable<Unit>? resort = null,
        int resetThreshold = 100)
        where TObj : notnull
        where TRhs : notnull
    {
        return obs.Sort(new Comparer<TObj, TRhs>(selector, comparer), options, resort, resetThreshold: resetThreshold);
    }
        
    public static IObservable<IChangeSet<TObj>> Sort<TObj, TRhs>(
        this IObservable<IChangeSet<TObj>> obs,
        Func<TObj, TRhs> selector,
        IObservable<IComparer<TRhs>> comparer,
        SortOptions options = SortOptions.None,
        IObservable<Unit>? resort = null,
        int resetThreshold = 100)
        where TObj : notnull
        where TRhs : notnull
    {
        return obs.Sort(comparer.Select(x => new Comparer<TObj, TRhs>(selector, x)), options, resort, resetThreshold: resetThreshold);
    }
}