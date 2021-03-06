using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Noggog.WPF
{
    public static class IViewForExt
    {
        public static IReactiveBinding<TView, TProp> OneWayBindStrict<TViewModel, TView, TProp>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp>> vmProperty,
            Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : class, IViewFor
        {
            return view.OneWayBind(
                viewModel: viewModel,
                vmProperty: vmProperty,
                viewProperty: viewProperty);
        }

        public static IReactiveBinding<TView, TOut> OneWayBindStrict<TViewModel, TView, TProp, TOut>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp>> vmProperty,
            Expression<Func<TView, TOut>> viewProperty,
            Func<TProp, TOut> selector)
            where TViewModel : class
            where TView : class, IViewFor
        {
            return view.OneWayBind(
                viewModel: viewModel,
                vmProperty: vmProperty,
                viewProperty: viewProperty,
                selector: selector);
        }

        public static IReactiveBinding<TView, (object? view, bool isViewModel)> BindStrict<TViewModel, TView, TProp>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp>> vmProperty,
            Expression<Func<TView, TProp>> viewProperty)
            where TViewModel : class
            where TView : class, IViewFor
        {
            return view.Bind(
                viewModel: viewModel,
                vmProperty: vmProperty,
                viewProperty: viewProperty);
        }

        public static IReactiveBinding<TView, (object? view, bool isViewModel)> BindStrict<TViewModel, TView, TVMProp, TVProp>(
            this TView view,
            TViewModel? viewModel,
            Expression<Func<TViewModel, TVMProp>> vmProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            Func<TVMProp, TVProp> vmToViewConverter,
            Func<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : class, IViewFor
        {
            return view.Bind(
                viewModel: viewModel,
                vmProperty: vmProperty,
                viewProperty: viewProperty,
                vmToViewConverter: vmToViewConverter,
                viewToVmConverter: viewToVmConverter);
        }

        public static IDisposable BindToStrict<TValue, TTarget>(
            this IObservable<TValue> @this,
            TTarget target,
            Expression<Func<TTarget, TValue>> property)
            where TTarget : class
        {
            return @this
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo<TValue, TTarget, TValue>(target, property);
        }

        public static IDisposable InvokeCommandStrict<T, TTarget, TRet>(this IObservable<T> item, TTarget target, Expression<Func<TTarget, ReactiveCommandBase<T, TRet>>> commandProperty)
            where TTarget : class
        {
            return ReactiveUI.ReactiveCommandMixins.InvokeCommand(item, target, commandProperty);
        }
    }
}
