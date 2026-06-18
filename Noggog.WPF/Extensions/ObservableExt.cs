using System.Reactive.Linq;
using System.Windows.Input;
using System.Reactive;
using System.Windows.Controls;

#nullable enable

namespace Noggog.WPF;

public static class ObservableExt
{
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
            control.SetValue(ControlsHelper.InErrorProperty, !x.Succeeded);
            control.SetValue(ControlsHelper.ErrorTooltipProperty, x.Reason);
        });
    }

    public static IDisposable BindError<T>(this IObservable<GetResponse<T>> err, Control control)
    {
        return err.Subscribe(x =>
        {
            control.SetValue(ControlsHelper.InErrorProperty, !x.Succeeded);
            control.SetValue(ControlsHelper.ErrorTooltipProperty, x.Reason);
        });
    }
    #endregion
}
