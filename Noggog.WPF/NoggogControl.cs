using System.Windows.Controls;

namespace Noggog.WPF;

public class NoggogControl : Control
{
    protected readonly IDisposableBucket _unloadDisposable = new DisposableBucket();
    protected readonly IDisposableBucket _templateDisposable = new DisposableBucket();

    public NoggogControl()
    {
        Loaded += (_, _) => OnLoaded();
        Loaded += (_, _) =>
        {
            if (Template != null)
            {
                OnApplyTemplate();
            }
        };
        Unloaded += (_, _) =>
        {
            _templateDisposable.Clear();
            _unloadDisposable.Clear();
        };
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _templateDisposable.Clear();
    }

    protected virtual void OnLoaded()
    {
    }
}