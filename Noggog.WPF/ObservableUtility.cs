using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Noggog.WPF
{
    public static class WPFObservableUtility
    {
        /// <summary>
        /// A convenience function to make a flip flop observable that triggers whenever the source observable changes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obs">Source observable</param>
        /// <param name="span">How long to delay before flopping back to false</param>
        /// <returns>Observable that turns true when source observable changes, then back to false after a time period</returns>
        public static IObservable<bool> FlipFlop<T>(IObservable<T> obs, TimeSpan span)
        {
            return Observable.Merge(
                obs.Select(f => true),
                obs.Delay(span).Select(f => false));
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
                keys: key.Single(),
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
    }
}
