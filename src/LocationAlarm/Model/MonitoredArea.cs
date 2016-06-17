using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace ArrivalAlarm.Model
{
    /// <summary>
    /// Contains all informations about monitored area 
    /// </summary>
    [DataContract]
    public class MonitoredArea
    {
        [DataMember]
        public BasicGeoposition? Geoposition { get; set; }

        [DataMember]
        public MonitoredGeofenceStates MonitoredStates { get; set; }

        [DataMember]
        public string Name { get; set; } = "Name";

        [DataMember]
        public double Radius { get; set; } = 500;

        public MonitoredArea()
        {
        }

        public MonitoredArea(MonitoredArea prototype)
        {
            Name = prototype.Name;
            Radius = prototype.Radius;
            Geoposition = prototype.Geoposition;
        }
    }
}