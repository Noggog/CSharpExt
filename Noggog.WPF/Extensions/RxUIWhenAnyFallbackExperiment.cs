using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;

namespace Noggog.WPF
{
    /// <summary>
    /// Temporary experiment files to add fallback functionality to RxUI.  This would more closely mimic XAML binding behavior.
    /// If it works out, this will be migrated to an official PR and pitched.
    /// </summary>
    public static class RxUIWhenAnyFallbackExperiment
    {
        private static readonly MemoizingMRUCache<(Type senderType, string propertyName, bool beforeChange), ICreatesObservableForProperty?> notifyFactoryCache =
            new MemoizingMRUCache<(Type senderType, string propertyName, bool beforeChange), ICreatesObservableForProperty?>(
                (t, _) =>
                {
                    return Locator.Current.GetServices<ICreatesObservableForProperty>()
                                  .Aggregate((score: 0, binding: (ICreatesObservableForProperty?)null), (acc, x) =>
                                  {
                                      int score = x.GetAffinityForObject(t.senderType, t.propertyName, t.beforeChange);
                                      return score > acc.score ? (score, x) : acc;
                                  }).binding;
                }, RxApp.BigCacheLimit);


        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <typeparam name="TSender">The sender type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="item">The source object to observe properties of.</param>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty.SomeOtherProperty'.</param>
        /// <param name="fallback">Value to use if any item on the expression chain is null.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <param name="skipInitial">If true, the Observable will not notify
        /// with the initial value.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        public static IObservable<IObservedChange<TSender, TValue>> ObservableForPropertyFallback<TSender, TValue>(
                this TSender item,
                Expression<Func<TSender, TValue>> property,
                TValue fallback,
                bool beforeChange = false,
                bool skipInitial = true)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            /* x => x.Foo.Bar.Baz;
             *
             * Subscribe to This, look for Foo
             * Subscribe to Foo, look for Bar
             * Subscribe to Bar, look for Baz
             * Subscribe to Baz, publish to Subject
             * Return Subject
             *
             * If Bar changes (notification fires on Foo), resubscribe to new Bar
             *  Resubscribe to new Baz, publish to Subject
             *
             * If Baz changes (notification fires on Bar),
             *  Resubscribe to new Baz, publish to Subject
             */

            return SubscribeToExpressionChain<TSender, TValue>(
                item,
                property.Body,
                fallback,
                beforeChange,
                skipInitial);
        }

        /// <summary>
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x => x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <param name="source">The object where we start the chain.</param>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <param name="fallback">Value to use if any item on the expression chain is null.</param>
        /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
        /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
        /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
        /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <returns>A observable which notifies about observed changes.</returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
            this TSender source,
            Expression expression,
            TValue fallback,
            bool beforeChange = false,
            bool skipInitial = true,
            bool suppressWarnings = false)
        {
            IObservable<IObservedChange<object?, object?>> notifier =
                Observable.Return(new ObservedChange<object?, object?>(null, null!, source));

            IEnumerable<Expression> chain = Reflection.Rewrite(expression).GetExpressionChain();
            notifier = chain.Aggregate(notifier, (n, expr) => n
                .Select(y => NestedObservedChanges(expr, y, beforeChange, suppressWarnings))
                .Switch());

            if (skipInitial)
            {
                notifier = notifier.Skip(1);
            }

            var r = notifier.Select(x =>
            {
                var val = x.Sender == null ? fallback : x.GetValue();

                // ensure cast to TValue will succeed, throw useful exception otherwise
                if (val != null && !(val is TValue))
                {
                    throw new InvalidCastException($"Unable to cast from {val.GetType()} to {typeof(TValue)}.");
                }

                return new ObservedChange<TSender, TValue>(source, expression, (TValue)val!);
            });

            return r.DistinctUntilChanged(x => x.Value);
        }

        private static IObservedChange<object?, object?> ObservedChangeFor(Expression expression, IObservedChange<object?, object?> sourceChange)
        {
            var propertyName = expression.GetMemberInfo().Name;
            if (sourceChange.Value == null)
            {
                return new ObservedChange<object?, object>(sourceChange.Value, expression);
            }

            // expression is always a simple expression
            Reflection.TryGetValueForPropertyChain(out object value, sourceChange.Value, new[] { expression });

            return new ObservedChange<object, object>(sourceChange.Value, expression, value);
        }

        private static IObservable<IObservedChange<object?, object?>> NestedObservedChanges(Expression expression, IObservedChange<object?, object?> sourceChange, bool beforeChange, bool suppressWarnings)
        {
            // Make sure a change at a root node propogates events down
            var kicker = ObservedChangeFor(expression, sourceChange);

            // Handle null values in the chain
            if (sourceChange.Value == null)
            {
                return Observable.Return(kicker);
            }

            // Handle non null values in the chain
            return NotifyForProperty(sourceChange.Value, expression, beforeChange, suppressWarnings)
                .Select(x => new ObservedChange<object?, object?>(x.Sender, expression, x.GetValue()))
                .StartWith(kicker);
        }

        private static IObservable<IObservedChange<object, object>>? NotifyForProperty(object sender, Expression expression, bool beforeChange, bool suppressWarnings)
        {
            var propertyName = expression.GetMemberInfo().Name;
            var result = notifyFactoryCache.Get((sender.GetType(), propertyName, beforeChange));

            if (result == null)
            {
                throw new Exception($"Could not find a ICreatesObservableForProperty for {sender.GetType()} property {propertyName}. This should never happen, your service locator is probably broken. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
            }

            return result.GetNotificationForProperty(sender, expression, propertyName, beforeChange, suppressWarnings);
        }

        /// <summary>
        /// WhenAnyValue allows you to observe whenever the value of a
        /// property on an object has changed, providing an initial value when
        /// the Observable is set up, unlike ObservableForProperty(). Use this
        /// method in constructors to set up bindings between properties that also
        /// need an initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyFallback<TSender, TRet>(
            this TSender This,
            Expression<Func<TSender, TRet>> property1,
            TRet fallback)
        {
            return This.WhenAnyFallback(property1, (IObservedChange<TSender, TRet> c1) => c1.Value, fallback);
        }

        public static IObservable<TRet> WhenAnyFallback<TSender, TRet>(
            this TSender This,
            Expression<Func<TSender, TRet>> property1)
        {
            return This.WhenAnyFallback(property1, (IObservedChange<TSender, TRet> c1) => c1.Value, default!);
        }

        /// <summary>
        /// WhenAny allows you to observe whenever one or more properties on an
        /// object have changed, providing an initial value when the Observable
        /// is set up, unlike ObservableForProperty(). Use this method in
        /// constructors to set up bindings between properties that also need an
        /// initial setup.
        /// </summary>
        public static IObservable<TRet> WhenAnyFallback<TSender, TRet, T1>(this TSender This,
                            Expression<Func<TSender, T1>> property1,
                            Func<IObservedChange<TSender, T1>, TRet> selector,
                            T1 fallback)
        {
            return This.ObservableForPropertyFallback(property1, fallback, false, false).Select(selector);
        }

        public static IObservable<TRet> WhenAnyFallback<TSender, TRet, T1>(this TSender This,
                            Expression<Func<TSender, T1>> property1,
                            Func<IObservedChange<TSender, T1>, TRet> selector)
        {
            return This.ObservableForPropertyFallback(property1, default!, false, false).Select(selector);
        }
    }
}
