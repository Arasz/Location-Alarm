using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    /// <summary>
    /// </summary>
    public class DayOfWeekToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var day = (DayOfWeek)value;
            return DateTimeFormatInfo.CurrentInfo.GetDayName(day);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var day = (string)value;
            return DateTime.Parse(day, CultureInfo.CurrentCulture).DayOfWeek;
        }
    }
}