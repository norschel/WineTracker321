using System;
using System.Globalization;
using System.Windows.Data;

namespace WineTracker.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class StarsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int rate)
                return new string('★', Math.Clamp(rate, 0, 5)) + new string('☆', Math.Clamp(5 - rate, 0, 5));
            return "☆☆☆☆☆";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
