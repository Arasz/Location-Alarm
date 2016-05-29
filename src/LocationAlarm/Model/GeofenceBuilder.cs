using System;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.Model
{
    public class GeofenceBuilder
    {
        /// <summary>
        /// The minimum time that a position has to be inside or outside of the Geofence in order for
        /// the notification to be triggered.
        /// </summary>
        public TimeSpan _dwellTime;

        /// <summary>
        /// The shape of the geofence region 
        /// </summary>
        public IGeoshape _geoshape;

        /// <summary>
        /// The id of the Geofence 
        /// </summary>
        public string _id;

        /// <summary>
        /// Indicates the states that the geofence is being monitored for 
        /// </summary>
        public MonitoredGeofenceStates _monitoredStates;

        /// <summary>
        /// Indicates whether the Geofence should be triggered once or multiple times. 
        /// </summary>
        public bool _singleUse;

        /// <summary>
        /// The time to start monitoring the Geofence. 
        /// </summary>
        public DateTimeOffset _startTime;

        /// <summary>
        /// Gets the time window, beginning after the StartTime, during which the Geofence is monitored 
        /// </summary>
        private TimeSpan _duration;

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        public double _radius { get; set; }
    }
}