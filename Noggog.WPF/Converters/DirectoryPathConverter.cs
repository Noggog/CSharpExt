using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Noggog.WPF
{
    [ValueConversion(typeof(DirectoryPath), typeof(string))]
    public class DirectoryPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException($"The target must be of type string");
            return ((DirectoryPath)value).Path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(DirectoryPath))
                throw new InvalidOperationException($"The target must be of type {nameof(DirectoryPath)}");
            return new DirectoryPath((string)value);
        }
    }
}
