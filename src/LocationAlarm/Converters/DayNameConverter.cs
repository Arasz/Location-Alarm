using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    public class DayNameConverter : IValueConverter
    {
        private string _everydayString = ResourceLoader.GetForCurrentView().GetString("Everyday");
        private string _repeatOnceString = ResourceLoader.GetForCurrentView().GetString("RepeatOnce");

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var activeDays = (IEnumerable<DayOfWeek>)value;
            if (!activeDays.Any())
                return _repeatOnceString;

            if (activeDays.Count() == 7)
                return _everydayString;

            return activeDays.OrderBy(week => week).Select(dayOfWeek => dayOfWeek.ToString().Substring(0, 3))
                    .Aggregate("", (aggregator, dayOfWeek) => aggregator += $"{dayOfWeek}, ")
                    .Trim(' ', ',');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}