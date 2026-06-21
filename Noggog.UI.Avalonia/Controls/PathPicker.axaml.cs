using Avalonia;
using Avalonia.Controls;

namespace Noggog.UI;

public partial class PathPicker : UserControl
{
    public static readonly StyledProperty<PathPickerVM?> PickerVMProperty =
        AvaloniaProperty.Register<PathPicker, PathPickerVM?>(nameof(PickerVM));

    public PathPickerVM? PickerVM
    {
        get => GetValue(PickerVMProperty);
        set => SetValue(PickerVMProperty, value);
    }

    public PathPicker()
    {
        InitializeComponent();
        // Bounce the picker VM onto the inner grid's DataContext so the markup can bind simply.
        this.GetObservable(PickerVMProperty).Subscribe(vm => RootGrid.DataContext = vm);
    }
}
