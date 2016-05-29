using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace ArrivalAlarm
{
    public class GeofenceChange
    {
        public Geofence Geofence { get; set; }

        public GeofenceState NewState { get; set; }
    }

    public class GeofenceService
    {
        private static GeofenceMonitor _geofenceMonitor = GeofenceMonitor.Current;

        /// <summary>
        /// Raised when state of one or more geofence object changed 
        /// </summary>
        public event EventHandler<GeofenceServiceEventArgs> GeofenceStateChanged;

        public Geoposition LastKnownGeopsition
        {
            get { return _geofenceMonitor.LastKnownGeoposition; }
        }

        public GeofenceService()
        {
            _geofenceMonitor.GeofenceStateChanged += _geofenceMonitor_GeofenceStateChanged;
            _geofenceMonitor.StatusChanged += _geofenceMonitor_StatusChanged;
        }

        public void Register(Geofence geofence)
        {
            _geofenceMonitor.Geofences.Add(geofence);
        }

        public void Remove(Geofence geofence)
        {
            _geofenceMonitor.Geofences.Remove(geofence);
        }

        protected virtual void OnGeofanceStateChanged(GeofenceServiceEventArgs e)
        {
            GeofenceStateChanged?.Invoke(this, e);
        }

        private void _geofenceMonitor_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var geofenceReports = sender.ReadReports();

            foreach (var geofenceReport in geofenceReports)
            {
                ;
            }
        }

        private void _geofenceMonitor_StatusChanged(GeofenceMonitor sender, object args)
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// </summary>
    public class GeofenceServiceEventArgs : EventArgs
    {
        public IEnumerable<Geofence> ChangedGeofences { get; private set; }

        public GeofenceServiceEventArgs(IEnumerable<Geofence> changedGeofence)
        {
            ChangedGeofences = changedGeofence;
        }
    }
}