using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace ArrivalAlarm.Model
{
    /// <summary>
    /// Contains all informations about monitored area 
    /// </summary>
    [DataContract]
    public struct MonitoredArea
    {
        [DataMember]
        public BasicGeoposition Geoposition { get; set; }

        [DataMember]
        public MonitoredGeofenceStates MonitoredStates { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public double Radius { get; set; }

        public MonitoredArea(double radius, string name = "", BasicGeoposition geoposition = default(BasicGeoposition),
            MonitoredGeofenceStates monitoredStates = MonitoredGeofenceStates.Entered)
        {
            Name = name;
            Radius = radius;
            Geoposition = geoposition;
            MonitoredStates = monitoredStates;
        }
    }
}