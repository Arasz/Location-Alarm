using Windows.Devices.Geolocation;

namespace LocationAlarm.Extensions
{
    public static class BasicGeopositionExtension
    {
        private static BasicGeoposition defaultGeoposition = default(BasicGeoposition);

        public static bool IsDefault(this BasicGeoposition geoposition)
        {
            return geoposition.Latitude == defaultGeoposition.Latitude &&
                geoposition.Altitude == defaultGeoposition.Altitude &&
                geoposition.Longitude == defaultGeoposition.Longitude;
        }
    }
}