using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Disposables;
using ReactiveUI;

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
    }
}
