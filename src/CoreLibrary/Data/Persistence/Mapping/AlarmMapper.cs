using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.DataModel;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;

namespace CoreLibrary.Data.Persistence.Mapping
{
    public class AlarmMapper : IMapper<Alarm, GeolocationAlarm>, IMapper<GeolocationAlarm, Alarm>
    {
        public GeolocationAlarm Map(Alarm source, GeolocationAlarm destination)
        {
            destination.ActiveDays = string.IsNullOrEmpty(source.ActiveDays) ? new List<WeekDay>() : source.ActiveDays
                .Split(',')
                .Select(name => new WeekDay(name))
                .ToList();

            destination.Geoposition = new BasicGeoposition
            {
                Longitude = source.Longitude,
                Altitude = source.Altitude,
                Latitude = source.Latitude,
            };
            return destination;
        }

        public Alarm Map(GeolocationAlarm source, Alarm destination)
        {
            destination.ActiveDays = source.ActiveDays
                .Select(day => day.Name)
                .Aggregate("", (accu, ele) => accu += $"{ele},")
                .TrimEnd(',');

            destination.Longitude = source.Geoposition.Longitude;
            destination.Altitude = source.Geoposition.Altitude;
            destination.Latitude = source.Geoposition.Latitude;

            return destination;
        }
    }
}