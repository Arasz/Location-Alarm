using System;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    internal class LongDayStringToShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var day = (DayOfWeek)value;
            return day.ToString().Substring(0, 3);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}