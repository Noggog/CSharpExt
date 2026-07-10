using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Noggog.UI;

public partial class PathPicker : UserControl
{
    public static readonly StyledProperty<PathPickerVM?> PickerVMProperty =
        AvaloniaProperty.Register<PathPicker, PathPickerVM?>(nameof(PickerVM));

    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<PathPicker, string?>(nameof(Watermark), defaultValue: "Path...");

    public static readonly StyledProperty<IBrush?> FieldBackgroundProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(FieldBackground), defaultValue: Brush.Parse("#101010"));

    public static readonly StyledProperty<IBrush?> FieldBackgroundPointerOverProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(FieldBackgroundPointerOver), defaultValue: Brush.Parse("#181818"));

    public static readonly StyledProperty<IBrush?> FieldBackgroundFocusedProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(FieldBackgroundFocused), defaultValue: Brush.Parse("#080808"));

    public static readonly StyledProperty<IBrush?> WarningBrushProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(WarningBrush), defaultValue: Brush.Parse("#FFC400"));

    public static readonly StyledProperty<IBrush?> GlyphBrushProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(GlyphBrush), defaultValue: Brush.Parse("#E0E0E0"));

    public static readonly StyledProperty<IBrush?> GlyphBrushPointerOverProperty =
        AvaloniaProperty.Register<PathPicker, IBrush?>(
            nameof(GlyphBrushPointerOver), defaultValue: Brush.Parse("#4AD9F2"));

    public PathPickerVM? PickerVM
    {
        get => GetValue(PickerVMProperty);
        set => SetValue(PickerVMProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public IBrush? FieldBackground
    {
        get => GetValue(FieldBackgroundProperty);
        set => SetValue(FieldBackgroundProperty, value);
    }

    public IBrush? FieldBackgroundPointerOver
    {
        get => GetValue(FieldBackgroundPointerOverProperty);
        set => SetValue(FieldBackgroundPointerOverProperty, value);
    }

    public IBrush? FieldBackgroundFocused
    {
        get => GetValue(FieldBackgroundFocusedProperty);
        set => SetValue(FieldBackgroundFocusedProperty, value);
    }

    public IBrush? WarningBrush
    {
        get => GetValue(WarningBrushProperty);
        set => SetValue(WarningBrushProperty, value);
    }

    public IBrush? GlyphBrush
    {
        get => GetValue(GlyphBrushProperty);
        set => SetValue(GlyphBrushProperty, value);
    }

    public IBrush? GlyphBrushPointerOver
    {
        get => GetValue(GlyphBrushPointerOverProperty);
        set => SetValue(GlyphBrushPointerOverProperty, value);
    }

    public PathPicker()
    {
        InitializeComponent();
        // Bounce the picker VM onto the inner grid's DataContext so the markup can bind simply.
        this.GetObservable(PickerVMProperty).Subscribe(vm => RootGrid.DataContext = vm);
    }
}
