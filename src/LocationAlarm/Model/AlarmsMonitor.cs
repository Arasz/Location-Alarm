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
            _locationAlarms[alarm.MonitoredArea.Id] = alarm;
            _geofenceMonitor.Geofences.Add((Geofence)alarm.MonitoredArea);
        }

        public void RemoveAlarm(AlarmModel alarm)
        {
            _locationAlarms.Remove(alarm.MonitoredArea.Id);
            _geofenceMonitor.Geofences.Remove((Geofence)alarm.MonitoredArea);
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