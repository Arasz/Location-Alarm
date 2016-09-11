using AutoMapper;
using CoreLibrary.Data.DataModel.PersistentModel;
using Windows.Devices.Geolocation;

namespace CoreLibrary.DataModel.Persistence.Mapping
{
    public class BasicGeopositionResolver : IValueResolver<Alarm, GeolocationAlarm, BasicGeoposition>
    {
        public BasicGeoposition Resolve(Alarm source, GeolocationAlarm destination, BasicGeoposition destMember,
            ResolutionContext context)
        {
            return new BasicGeoposition
            {
                Longitude = source.Longitude,
                Altitude = source.Altitude,
                Latitude = source.Latitude,
            };
        }
    }
}