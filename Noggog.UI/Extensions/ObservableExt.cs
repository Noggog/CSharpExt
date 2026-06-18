using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Noggog.UI.Containers;

#nullable enable

namespace Noggog.UI;

public static class ObservableExt
{
    public static ObservableAsPropertyHelper<TRet> ToGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        TRet initialValue,
        IScheduler scheduler,
        bool deferSubscription = false)
    {
        return source
            .ToProperty(vm, property, initialValue, deferSubscription: deferSubscription, scheduler: scheduler)
            .DisposeWith(vm);
    }

    public static ObservableAsPropertyHelper<TRet> ToRxAppGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        TRet initialValue,
        bool deferSubscription = false)
    {
        return source.ToGuiProperty(vm, property, initialValue, RxApp.MainThreadScheduler, deferSubscription);
    }

    public static ObservableAsPropertyHelper<TRet> ToGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        IScheduler scheduler,
        bool deferSubscription = false)
        where TRet : struct
    {
        return source
            .ToProperty(vm, property, initialValue: default!, deferSubscription, scheduler)
            .DisposeWith(vm);
    }

    public static ObservableAsPropertyHelper<TRet> ToRxAppGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        bool deferSubscription = false)
        where TRet : struct
    {
        return source.ToGuiProperty(vm, property, RxApp.MainThreadScheduler, deferSubscription);
    }

    public static void ToGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        TRet initialValue,
        out ObservableAsPropertyHelper<TRet> result,
        IScheduler scheduler,
        bool deferSubscription = false)
    {
        OAPHCreationHelperMixin.ToProperty(
                target: source,
                source: vm,
                property: property,
                getInitialValue: () => initialValue,
                result: out result,
                deferSubscription: deferSubscription,
                scheduler: scheduler)
            .DisposeWith(vm);
    }

    public static void ToRxAppGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        TRet initialValue,
        out ObservableAsPropertyHelper<TRet> result,
        bool deferSubscription = false)
    {
        source.ToGuiProperty(vm, property, initialValue, out result, RxApp.MainThreadScheduler, deferSubscription);
    }

    public static void ToGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        Func<TRet> getInitialValue,
        out ObservableAsPropertyHelper<TRet> result,
        IScheduler scheduler,
        bool deferSubscription = false)
    {
        source.ToProperty(source: vm, property: property, result: out result, getInitialValue: getInitialValue, deferSubscription: deferSubscription, scheduler: scheduler)
            .DisposeWith(vm);
    }

    public static void ToRxAppGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        Func<TRet> getInitialValue,
        out ObservableAsPropertyHelper<TRet> result,
        bool deferSubscription = false)
    {
        source.ToGuiProperty(vm, property, getInitialValue, out result, RxApp.MainThreadScheduler, deferSubscription);
    }

    public static void ToGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        out ObservableAsPropertyHelper<TRet> result,
        IScheduler scheduler,
        bool deferSubscription = false)
        where TRet : struct
    {
        source.ToProperty(vm, property, out result, getInitialValue: () => default, deferSubscription, scheduler)
            .DisposeWith(vm);
    }

    public static void ToRxAppGuiProperty<TRet>(
        this IObservable<TRet> source,
        ViewModel vm,
        string property,
        out ObservableAsPropertyHelper<TRet> result,
        bool deferSubscription = false)
        where TRet : struct
    {
        source.ToGuiProperty(vm, property, out result, RxApp.MainThreadScheduler, deferSubscription);
    }

    public static IObservable<T> ObserveOnRxAppGui<T>(this IObservable<T> obs)
    {
        return obs.ObserveOn(RxApp.MainThreadScheduler);
    }

    public static IDisposable Subscribe<T>(this IObservable<T> obs, Action onCompleted)
    {
        return obs.Subscribe(onNext: (t) => { }, onCompleted: onCompleted);
    }

    public static IObservable<IChangeSet<T>> Bind<T>(this IObservable<IChangeSet<T>> source, out IObservableCollection<T> readOnlyObservableCollection, int resetThreshold = 25)
        where T : notnull
    {
        var obsCol = new ObservableCollectionExtended<T>();
        readOnlyObservableCollection = obsCol;
        return source.Bind(obsCol, resetThreshold);
    }

    public static IObservableCollection<TObj> ToObservableCollection<TObj>(this IObservable<IChangeSet<TObj>> changeSet, IDisposableDropoff disposable, IScheduler scheduler)
        where TObj : notnull
    {
        changeSet
            .ObserveOn(scheduler)
            .Bind(out IObservableCollection<TObj> display)
            .Subscribe()
            .DisposeWith(disposable);
        return display;
    }

    public static IObservableCollection<TObj> ToRxAppObservableCollection<TObj>(this IObservable<IChangeSet<TObj>> changeSet, IDisposableDropoff disposable)
        where TObj : notnull
    {
        return changeSet.ToObservableCollection(disposable, RxApp.MainThreadScheduler);
    }

    public static IObservableCollection<TObj> ToObservableCollection<TObj, TKey>(this IObservable<IChangeSet<TObj, TKey>> changeSet, IDisposableDropoff disposable, IScheduler scheduler)
        where TObj : notnull
        where TKey : notnull
    {
        ObservableCollectionExtended<TObj> display = new ObservableCollectionExtended<TObj>();
        changeSet
            .ObserveOn(scheduler)
            .Bind(display)
            .Subscribe()
            .DisposeWith(disposable);
        return display;
    }

    public static IObservableCollection<TObj> ToRxAppObservableCollection<TObj, TKey>(this IObservable<IChangeSet<TObj, TKey>> changeSet, IDisposableDropoff disposable)
        where TObj : notnull
        where TKey : notnull
    {
        return changeSet.ToObservableCollection(disposable, RxApp.MainThreadScheduler);
    }

    public static IDisposable WireSelectionTracking<TItem>(this IObservable<TItem?> obs)
        where TItem : class, ISelectable
    {
        return obs
            .StartWith(default(TItem))
            .Pairwise()
            .Subscribe(x =>
            {
                if (x.Previous != null)
                {
                    x.Previous.IsSelected = false;
                }

                if (x.Current != null)
                {
                    x.Current.IsSelected = true;
                }
            });
    }

    public static IObservable<IChangeSet<SelectedVm<T>>> WrapInSelectedCollection<T>(
        this IObservable<IList<T>?> list,
        out ReadOnlyObservableCollection<SelectedVm<T>> selectedList)
        where T : notnull
    {
        return list.Select(x =>
            {
                if (x is ObservableCollection<T> obsCollection)
                {
                    return obsCollection.ToObservableChangeSet();
                }

                if (x is IObservableCollection<T> obsCollInterf)
                {
                    return obsCollInterf.ToObservableChangeSet<IObservableCollection<T>, T>();
                }

                return Observable.Empty<IChangeSet<T>>();
            })
            .Switch()
            .Transform(x => new SelectedVm<T>(x))
            .Bind(out selectedList);
    }

    public static IDisposable WrapInDerivativeSelectedCollection<T>(
        this IObservable<IList<T>?> list,
        out IDerivativeSelectedCollection<T> selectedList)
        where T : notnull
    {
        var derivativeList = new DerivativeSelectedCollection<T>();
        selectedList = derivativeList;
        var ret = WrapInSelectedCollection(
                list
                    .Do(x => derivativeList.OriginalList = x),
                out var readOnlyList)
            .Subscribe();
        derivativeList.DerivativeList = readOnlyList;
        return ret;
    }
}
