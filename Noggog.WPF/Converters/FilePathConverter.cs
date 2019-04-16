using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Noggog.WPF
{
    [ValueConversion(typeof(FilePath), typeof(string))]
    public class FilePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new InvalidOperationException($"The target must be of type string");
            return ((FilePath)value).Path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(FilePath))
                throw new InvalidOperationException($"The target must be of type {nameof(FilePath)}");
            return new FilePath((string)value);
        }
    }
}
