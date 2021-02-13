using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Noggog.WPF
{
    [ValueConversion(typeof(Visibility), typeof(bool))]
    public class VisibilityConverter : IValueConverter
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
            return ((bool)value) == compareTo ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
