using System.Windows;
using System.Windows.Controls.Primitives;

namespace Noggog.WPF;

public class ControlsHelper
{
    public static readonly DependencyProperty InErrorProperty = DependencyProperty.RegisterAttached("InError", typeof(bool), typeof(ControlsHelper), new UIPropertyMetadata(false));

    [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
    public static bool GetInError(DependencyObject obj)
    {
        return (bool)obj.GetValue(InErrorProperty);
    }

    [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
    public static void SetInError(DependencyObject obj, bool value)
    {
        obj.SetValue(InErrorProperty, value);
    }

    public static readonly DependencyProperty ErrorTooltipProperty = DependencyProperty.RegisterAttached("ErrorTooltip", typeof(string), typeof(ControlsHelper), new UIPropertyMetadata(string.Empty));

    [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
    public static string GetErrorTooltip(DependencyObject obj)
    {
        return (string)obj.GetValue(ErrorTooltipProperty);
    }

    [AttachedPropertyBrowsableForType(typeof(TextBoxBase))]
    public static void SetErrorTooltip(DependencyObject obj, string value)
    {
        obj.SetValue(ErrorTooltipProperty, value);
    }
}