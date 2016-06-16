using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Geofence class decorator 
    /// </summary>
    [DataContract]
    public class MonitoredArea
    {
        /// <summary>
        /// Decorated geofence object 
        /// </summary>
        [DataMember]
        private Geofence _geofence;

        /// <summary>
        /// Gets the time window, beginning after the StartTime, during which the Geofence is monitored 
        /// </summary>
        [DataMember]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The minimum time that a position has to be inside or outside of the Geofence in order for
        /// the notification to be triggered.
        /// </summary>
        [DataMember]
        public TimeSpan DwellTime { get; set; }

        public Geofence Geofence
        {
            get
            {
                if (_geofence == null)
                {
                    var builder = new GeofenceBuilder()
                        .SetRequiredId(Id)
                        .ThenSetGeocircle(Geopoint.Position, Radius)
                        .SetDwellTime(DwellTime)
                        .SetStartTime(StartTime)
                        .ConfigureMonitoredStates(MonitoredStates);
                    if (SingleUse)
                        builder.UseOnlyOnce();
                    _geofence = builder.Build();
                }

                return _geofence;
            }
        }

        /// <summary>
        /// Alarm position on map 
        /// </summary>
        [DataMember]
        public Geopoint Geopoint { get; set; }

        /// <summary>
        /// The shape of the geofence region 
        /// </summary>
        public IGeoshape Geoshape => _geofence.Geoshape;

        /// <summary>
        /// The id of the Geofence 
        /// </summary>
        public string Id => Name;

        [ForeignKey(typeof(global::LocationAlarm.Model.LocationAlarm))]
        public int LocationAlarmId { get; set; }

        /// <summary>
        /// Indicates the states that the geofence is being monitored for 
        /// </summary>
        public MonitoredGeofenceStates MonitoredStates => _geofence.MonitoredStates;

        /// <summary>
        /// Monitored area name 
        /// </summary>
        [DataMember, PrimaryKey]
        public string Name { get; set; } = "Name";

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        [DataMember]
        public double Radius { get; set; } = 500;

        /// <summary>
        /// Indicates whether the Geofence should be triggered once or multiple times. 
        /// </summary>
        [DataMember]
        public bool SingleUse { get; set; }

        /// <summary>
        /// The time to start monitoring the Geofence. 
        /// </summary>
        [DataMember]
        public DateTimeOffset StartTime { get; set; }

        public MonitoredArea()
        {
        }

        public MonitoredArea(MonitoredArea prototype)
        {
            Name = prototype.Name;
            Radius = prototype.Radius;
            SingleUse = prototype.SingleUse;
            StartTime = prototype.StartTime;
            Geopoint = prototype.Geopoint;
            DwellTime = prototype.DwellTime;
            Duration = prototype.Duration;
        }

        public static explicit operator Geofence(MonitoredArea monitoredArea)
        {
            return monitoredArea.Geofence;
        }
    }
}