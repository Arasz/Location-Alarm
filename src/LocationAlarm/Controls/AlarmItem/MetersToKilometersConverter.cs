using System;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Controls.AlarmItem
{
    /// <summary>
    /// </summary>
    internal class MetersToKilometersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            //BUG: SLOW CODE ! BOXING!
            var meters = (double)value;
            return meters / 1000;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}