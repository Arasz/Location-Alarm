using System;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    internal class GeopositionToGeopointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return value;
            var geposition = (BasicGeoposition?)value;
            return new Geopoint(geposition.Value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return value;
            var geopoint = (Geopoint)value;
            return geopoint.Position;
        }
    }
}