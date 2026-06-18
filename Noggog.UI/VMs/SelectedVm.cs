using ReactiveUI.Fody.Helpers;

namespace Noggog.UI;

public class SelectedVm<T> : ViewModel, ISelectedItem<T>
{
    [Reactive] public bool IsSelected { get; set; }
    [Reactive] public T Item { get; set; }

    public SelectedVm(T initialItem)
    {
        Item = initialItem;
    }
}