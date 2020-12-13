using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive.Disposables;
using ReactiveUI;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using System.Windows.Input;
using System.Reactive;
using System.Windows.Controls;
#nullable enable

namespace Noggog.WPF
{
    public static class ObservableExt
    {
        public static T DisposeWith<T>(this T item, ViewModel vm)
            where T : IDisposable
        {
            item.DisposeWith(vm.CompositeDisposable);
            return item;
        }

        public static ObservableAsPropertyHelper<TRet> ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            TRet initialValue,
            bool deferSubscription = false)
        {
            return source
                .ToProperty(vm, property, initialValue, deferSubscription: deferSubscription, scheduler: RxApp.MainThreadScheduler)
                .DisposeWith(vm.CompositeDisposable);
        }

        public static ObservableAsPropertyHelper<TRet> ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            bool deferSubscription = false)
            where TRet : struct
        {
            return source
                .ToProperty(vm, property, initialValue: default!, deferSubscription, RxApp.MainThreadScheduler)
                .DisposeWith(vm.CompositeDisposable);
        }

        public static void ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            TRet initialValue,
            out ObservableAsPropertyHelper<TRet> result,
            bool deferSubscription = false)
        {
            source.ToProperty(vm, property, out result, initialValue, deferSubscription, RxApp.MainThreadScheduler)
                .DisposeWith(vm.CompositeDisposable);
        }

        public static void ToGuiProperty<TRet>(
            this IObservable<TRet> source,
            ViewModel vm,
            string property,
            out ObservableAsPropertyHelper<TRet> result,
            bool deferSubscription = false)
            where TRet : struct
        {
            source.ToProperty(vm, property, out result, initialValue: default, deferSubscription, RxApp.MainThreadScheduler)
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

        public static IObservableCollection<TObj> ToObservableCollection<TObj>(this IObservable<IChangeSet<TObj>> changeSet, CompositeDisposable disposable)
        {
            changeSet
                .ObserveOnGui()
                .Bind(out IObservableCollection<TObj> display)
                .Subscribe()
                .DisposeWith(disposable);
            return display;
        }

        public static IObservableCollection<TObj> ToObservableCollection<TObj, TKey>(this IObservable<IChangeSet<TObj, TKey>> changeSet, CompositeDisposable disposable)
        {
            ObservableCollectionExtended<TObj> display = new ObservableCollectionExtended<TObj>();
            changeSet
                .ObserveOnGui()
                .Bind(display)
                .Subscribe()
                .DisposeWith(disposable);
            return display;
        }

        #region Keybinds
        public static IObservable<Unit> Keybind(
            this IObservable<KeyEventArgs> events,
            Key key,
            ModifierKeys modifiers = default)
        {
            return Keybind_Internal(
                triggeringEvents: events
                    .Where(e => e.Key == key),
                keys: key.AsEnumerable(),
                modifiers: modifiers);
        }

        public static IObservable<Unit> Keybind(
            this IObservable<KeyEventArgs> events,
            ModifierKeys? modifiers,
            params Key[] keys)
        {
            return Keybind(
                events: events,
                keys: (IEnumerable<Key>)keys,
                modifiers: modifiers);

        }

        public static IObservable<Unit> Keybind(
            this IObservable<KeyEventArgs> events,
            IEnumerable<Key> keys,
            ModifierKeys? modifiers = null)
        {
            if (!keys.Any())
            {
                throw new ArgumentException("Keys cannot be empty");
            }
            HashSet<Key> triggerKeys = new HashSet<Key>(keys);
            return Keybind_Internal(
                triggeringEvents: events
                    .Where(e => triggerKeys.Contains(e.Key)),
                keys: keys,
                modifiers: modifiers);

        }

        private static IObservable<Unit> Keybind_Internal(
            IObservable<KeyEventArgs> triggeringEvents,
            IEnumerable<Key> keys,
            ModifierKeys? modifiers)
        {
            return triggeringEvents
                .Where(u =>
                {
                    if (modifiers.HasValue && modifiers.Value != Keyboard.Modifiers) return false;
                    foreach (var key in keys)
                    {
                        if (key != u.Key && !Keyboard.IsKeyDown(key)) return false;
                    }
                    return true;
                })
                .Select(e => Unit.Default);
        }
        #endregion

        #region ErrorBinding
        public static IDisposable BindError(this IObservable<ErrorResponse> err, Control control)
        {
            return err.Subscribe(x =>
            {
                control.SetValue(Noggog.WPF.ControlsHelper.InErrorProperty, !x.Succeeded);
                control.SetValue(Noggog.WPF.ControlsHelper.ErrorTooltipProperty, x.Reason);
            });
        }

        public static IDisposable BindError<T>(this IObservable<GetResponse<T>> err, Control control)
        {
            return err.Subscribe(x =>
            {
                control.SetValue(Noggog.WPF.ControlsHelper.InErrorProperty, !x.Succeeded);
                control.SetValue(Noggog.WPF.ControlsHelper.ErrorTooltipProperty, x.Reason);
            });
        }
        #endregion
    }
}
