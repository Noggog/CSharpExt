using System.Globalization;
using System.Windows.Data;

namespace Noggog.WPF;

public class EnumDescriptionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (targetType != typeof(string))
            throw new InvalidOperationException($"The target must be of type string");
        return EnumExt.ToDescriptionString((Enum)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}