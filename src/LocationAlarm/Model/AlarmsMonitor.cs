using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.Model
{
    /// <summary>
    /// </summary>
    internal class AlarmsMonitor
    {
        private GeofenceMonitor _geofenceMonitor = GeofenceMonitor.Current;

        private IDictionary<string, AlarmModel> _locationAlarms;

        public AlarmsMonitor()
        {
            _locationAlarms = new Dictionary<string, AlarmModel>();

            _geofenceMonitor.GeofenceStateChanged += GeofenceMonitorOnGeofenceStateChanged;
            _geofenceMonitor.StatusChanged += GeofenceMonitorOnStatusChanged;
        }

        public void AddAlarm(AlarmModel alarm)
        {
            _locationAlarms[alarm.MonitoredLocation.Id] = alarm;
            _geofenceMonitor.Geofences.Add(alarm.MonitoredLocation);
        }

        public void RemoveAlarm(ILocationAlarmAlarmModel alarm)
        {
            _locationAlarms.Remove(alarm.LocationMarker.Id);
            _geofenceMonitor.Geofences.Remove(alarm.LocationMarker);
        }

        private void ChoseActionBasedOnState(Geofence geofence, GeofenceState newState)
        {
            switch (newState)
            {
                case GeofenceState.None:
                    break;

                case GeofenceState.Entered:
                    _locationAlarms[geofence.Id].Activate();
                    break;

                case GeofenceState.Exited:
                    _locationAlarms[geofence.Id].Deactivate();
                    break;

                case GeofenceState.Removed:
                    RemoveAlarm(_locationAlarms[geofence.Id]);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
            }
        }

        private void GeofenceMonitorOnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var reports = sender.ReadReports();

            foreach (var geofenceStateChangeReport in reports)
            {
                ChoseActionBasedOnState(geofenceStateChangeReport.Geofence, geofenceStateChangeReport.NewState);
            }
        }

        private void GeofenceMonitorOnStatusChanged(GeofenceMonitor sender, object args)
        {
        }
    }
}