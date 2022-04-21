using System.Windows;
using System.Windows.Controls;

namespace Noggog.WPF;

/// <summary>
/// Interaction logic for PathPicker.xaml
/// </summary>
public partial class PathPicker : UserControl
{
    // This exists, as utilizing the datacontext directly seemed to bug out the exit animations
    // "Bouncing" off this property seems to fix it, though.  Could perhaps be done other ways.
    public PathPickerVM PickerVM
    {
        get => (PathPickerVM)GetValue(PickerVMProperty);
        set => SetValue(PickerVMProperty, value);
    }
    public static readonly DependencyProperty PickerVMProperty = DependencyProperty.Register(nameof(PickerVM), typeof(PathPickerVM), typeof(PathPicker),
        new FrameworkPropertyMetadata(default(PathPickerVM)));

    public PathPicker()
    {
        InitializeComponent();
    }
}