using System;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.Model
{
    public class GeofenceBuilder
    {
        /// <summary>
        /// Gets the time window, beginning after the StartTime, during which the Geofence is monitored 
        /// </summary>
        private TimeSpan? _duration;

        /// <summary>
        /// The minimum time that a position has to be inside or outside of the Geofence in order for
        /// the notification to be triggered.
        /// </summary>
        private TimeSpan? _dwellTime;

        /// <summary>
        /// The shape of the geofence region 
        /// </summary>
        private Geocircle _geoshape = new Geocircle(new BasicGeoposition(), 4d);

        /// <summary>
        /// The id of the Geofence 
        /// </summary>
        private string _id = string.Empty;

        /// <summary>
        /// Indicates the states that the geofence is being monitored for 
        /// </summary>
        private MonitoredGeofenceStates? _monitoredStates;

        /// <summary>
        /// Indicates whether the Geofence should be triggered once or multiple times. 
        /// </summary>
        private bool _singleUse;

        /// <summary>
        /// The time to start monitoring the Geofence. 
        /// </summary>
        private DateTimeOffset? _startTime;

        public GeofenceBuilder()
        {
        }

        public GeofenceBuilder(Geofence prototype)
        {
            SetRequiredId(prototype.Id);
            ThenSetGeocircle(prototype.Geoshape);
            SetStartTime(prototype.StartTime);
            SetDwellTime(prototype.DwellTime);
            if (prototype.SingleUse)
                UseOnlyOnce();
        }

        public Geofence Build()
        {
            if (_monitoredStates == null)
                return new Geofence(_id, _geoshape);
            if (_dwellTime == null)
                return new Geofence(_id, _geoshape, _monitoredStates.Value, _singleUse);
            if (_startTime == null || _duration == null)
                return new Geofence(_id, _geoshape, _monitoredStates.Value, _singleUse, _dwellTime.Value);

            return new Geofence(_id, _geoshape, _monitoredStates.Value, _singleUse, _dwellTime.Value, _startTime.Value, _duration.Value);
        }

        public GeofenceBuilder ConfigureMonitoredStates(MonitoredGeofenceStates states)
        {
            _monitoredStates = states;
            return this;
        }

        public GeofenceBuilder SetDwellTime(TimeSpan dwellTime)
        {
            _dwellTime = dwellTime;
            return this;
        }

        public GeofenceBuilder SetRequiredId(string id)
        {
            _id = id;
            return this;
        }

        public GeofenceBuilder SetStartTime(DateTimeOffset startTime)
        {
            _startTime = startTime;
            return this;
        }

        public GeofenceBuilder ThenSetGeocircle(IGeoshape geocricle)
        {
            if (geocricle.GeoshapeType != GeoshapeType.Geocircle)
                throw new ArgumentException($"{nameof(GeofenceBuilder)} can use only {nameof(Geocircle)} as {nameof(IGeoshape)}");

            _geoshape = geocricle as Geocircle;
            return this;
        }

        public GeofenceBuilder ThenSetGeocircle(BasicGeoposition position, double radius)
        {
            _geoshape = new Geocircle(position, radius);
            return this;
        }

        public GeofenceBuilder UseOnlyOnce()
        {
            _singleUse = true;
            return this;
        }
    }
}