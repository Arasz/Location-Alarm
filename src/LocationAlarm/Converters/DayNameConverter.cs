using CoreLibrary.Data;
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
            if (null == value) return value;
            var activeDays = (IEnumerable<WeekDay>)value;
            if (!activeDays.Any())
                return _repeatOnceString;

            if (activeDays.Count() == 7)
                return _everydayString;

            return activeDays.OrderBy(day => day.Day).Select(day => day.ShortName)
                    .Aggregate("", (aggregator, day) => aggregator += $"{day}, ")
                    .Trim(' ', ',');
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}