using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Disposables;
using ReactiveUI;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
#nullable enable

namespace Noggog.WPF
{
    public static class ObservableExt
    {
        public static void DisposeWith<T>(this T item, ViewModel vm)
            where T : IDisposable
        {
            item.DisposeWith(vm.CompositeDisposable);
        }

        public static ObservableAsPropertyHelper<TRet> ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            TRet initialValue = default,
            bool deferSubscription = false)
        {
            return source
                .ToProperty(vm, property, initialValue, deferSubscription, RxApp.MainThreadScheduler)
                .DisposeWith(vm.CompositeDisposable);
        }

        public static void ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            out ObservableAsPropertyHelper<TRet> result,
            TRet initialValue = default,
            bool deferSubscription = false)
        {
            source.ToProperty(vm, property, out result, initialValue, deferSubscription, RxApp.MainThreadScheduler)
                .DisposeWith(vm.CompositeDisposable);
        }

        public static IObservable<T> ObserveOnGui<T>(this IObservable<T> obs)
        {
            return obs.ObserveOn(RxApp.MainThreadScheduler);
        }

        public static IDisposable Subscribe<T>(this IObservable<T> obs, Action onCompleted)
        {
            return obs.Subscribe(onNext: (t) => { }, onCompleted: onCompleted);
        }

        public static IObservable<IChangeSet<T>> Bind<T>(this IObservable<IChangeSet<T>> source, out IObservableCollection<T> readOnlyObservableCollection, int resetThreshold = 25)
        {
            var obsCol = new ObservableCollectionExtended<T>();
            readOnlyObservableCollection = obsCol;
            return source.Bind(obsCol, resetThreshold);
        }

        public static IObservableCollection<TObj> ToObservableCollection<TObj>(this IObservable<IChangeSet<TObj>> changeSet, CompositeDisposable dispoosable)
        {
            changeSet
                .ObserveOnGui()
                .Bind(out IObservableCollection<TObj> display)
                .Subscribe()
                .DisposeWith(dispoosable);
            return display;
        }
    }
}
