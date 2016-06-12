using LocationAlarm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    public class AlarmTypeConverter : IValueConverter
    {
        private readonly Dictionary<string, AlarmType> _alarmTypeMap = new Dictionary<string, AlarmType>();

        private IEnumerable<AlarmType> AlarmTypes { get; } = Enum.GetValues(typeof(AlarmType)).Cast<AlarmType>();

        private ResourceLoader Loader { get; } = ResourceLoader.GetForCurrentView();

        public AlarmTypeConverter()
        {
            AlarmTypes.ForEach(type => _alarmTypeMap[Loader.GetString(type.ToString())] = type);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var alarmType = (AlarmType)value;
            return Loader.GetString(alarmType.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}