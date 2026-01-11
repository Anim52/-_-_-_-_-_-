using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MaxiMed.Wpf.Converters
{
    public sealed class BoolToFreeBusyBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isFree)
                return isFree
                    ? new SolidColorBrush(Color.FromRgb(220, 255, 220)) 
                    : new SolidColorBrush(Color.FromRgb(255, 220, 220)); 

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
