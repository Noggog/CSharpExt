using ReactiveUI;

namespace Noggog.WPF;

public class NoggogUserControl<TViewModel> : ReactiveUserControl<TViewModel>
    where TViewModel : class
{
    public NoggogUserControl()
    {
        DataContextChanged += (o, e) =>
        {
            if (e.NewValue is TViewModel vm)
            {
                ViewModel = vm;
            }
            else
            {
                ViewModel = null!;
            }
        };
    }
}