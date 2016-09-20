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
            return targetType == typeof(string) ? ConvertString(value as string) : ConvertList(value as IEnumerable<string>);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private string ConvertList(IEnumerable<string> list)
        {
            var activeDays = (list).ToList();

            if (!activeDays.Any())
                return _repeatOnceString;

            if (activeDays.Count() == 7)
                return _everydayString;

            return activeDays
                .Select(day => day.Substring(0, 3))
                .Aggregate("", (aggregator, day) => aggregator += $"{day}, ")
                .Trim(' ', ',');
        }

        private string ConvertString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return _repeatOnceString;

            var days = value.Split(',');

            if (days.Length == 7)
                return _everydayString;

            return days.Select(day => $"{day.Substring(0, 3)},")
                .Aggregate("", (accu, day) => accu += day)
                .TrimEnd(',');
        }
    }
}