using System.Linq;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;

namespace LocationAlarm.Model
{
    /// <summary>
    /// </summary>
    [DataContract]
    public class AlarmLocation
    {
        /// <summary>
        /// </summary>
        [DataMember]
        public BasicGeoposition Geoposition { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public double Radius { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="name"> Location <c> name </c> </param>
        /// <param name="geoposition"> Position on map in geographic coordinates </param>
        /// <param name="geocircleRadius"> Geocircle (geofence) radius in meters </param>
        public AlarmLocation(string name, BasicGeoposition geoposition, double geocircleRadius = 4d)
        {
            Name = name;
            Geoposition = geoposition;
            Radius = geocircleRadius;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode() + Radius.GetHashCode() + Geoposition.GetHashCode();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}\n" +
                $"{nameof(Geocircle.Center).Split('.').Last()}: Alt: {Geoposition.Altitude}, Lat: {Geoposition.Latitude}, Long: {Geoposition.Longitude}\n" +
                $"{nameof(Geocircle.Radius).Split('.').Last()}: {Radius}\n";
        }
    }
}