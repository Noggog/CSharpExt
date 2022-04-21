using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Noggog.WPF;

public static class TextBoxExt
{
    private static void IgnoreMouseButton(object sender, MouseButtonEventArgs e)
    {
        var textBox = sender as TextBox;
        if (textBox == null || (!textBox.IsReadOnly && textBox.IsKeyboardFocusWithin)) return;

        e.Handled = true;
        textBox.Focus();
    }

    public static IObservable<Unit> GotFocusWithSelectionSkipped(this TextBox textBox)
    {
        return Observable.Create<Unit>(obs =>
        {
            CompositeDisposable disp = new CompositeDisposable();

            Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => textBox.PreviewMouseDown += h, h => textBox.PreviewMouseDown -= h)
                .Subscribe(e =>
                {
                    var textBox = e.Sender as TextBox;
                    if (textBox == null || (!textBox.IsReadOnly && textBox.IsKeyboardFocusWithin)) return;

                    e.EventArgs.Handled = true;
                    textBox.Focus();
                })
                .DisposeWith(disp);

            textBox.Events().GotFocus
                .Subscribe(_ =>
                {
                    obs.OnNext(Unit.Default);
                })
                .DisposeWith(disp);

            return disp;
        });
    }
}