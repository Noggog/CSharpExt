using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Noggog.WPF;

[ValueConversion(typeof(Visibility), typeof(object))]
public class IsNotNullVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(Visibility))
            throw new InvalidOperationException($"The target must be of type {nameof(Visibility)}");
        bool compareTo = true;
        if (parameter is bool p)
        {
            compareTo = p;
        }
        else if (parameter is string str && str.Equals("FALSE", StringComparison.OrdinalIgnoreCase))
        {
            compareTo = false;
        }
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}