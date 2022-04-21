using System.Windows;
using System.Windows.Controls;

namespace Noggog.WPF;

public class ErroredTextBox : TextBox
{
    public bool InError
    {
        get => (bool)GetValue(InErrorProperty);
        set => SetValue(InErrorProperty, value);
    }
    public static readonly DependencyProperty InErrorProperty = DependencyProperty.Register(nameof(InError), typeof(bool), typeof(ErroredTextBox),
        new FrameworkPropertyMetadata(default(bool)));

    public string ErrorText
    {
        get => (string)GetValue(ErrorTextProperty);
        set => SetValue(ErrorTextProperty, value);
    }
    public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register(nameof(ErrorText), typeof(string), typeof(ErroredTextBox),
        new FrameworkPropertyMetadata(default(string)));
}