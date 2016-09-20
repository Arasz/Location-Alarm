using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation.Geofencing;

namespace CoreLibrary.Service.Geofencing
{
    public class GeofenceService : IGeofenceService
    {
        public IEnumerable<Geofence> AllActiveGeofences => GeofenceMonitor.Geofences.ToList();

        public IReadOnlyList<GeofenceStateChangeReport> GeofenceStateChangeReports => GeofenceMonitor.ReadReports().ToList();

        public GeofenceMonitorStatus Status => GeofenceMonitor.Status;

        private GeofenceMonitor GeofenceMonitor => GeofenceMonitor.Current;

        public bool IsGeofenceRegistered(string id) => GeofenceMonitor.Geofences.Any(geofence => geofence.Id == id);

        public Geofence ReadGeofence(string id) => GeofenceMonitor.Geofences.FirstOrDefault(geofence => geofence.Id == id);

        public void RegisterGeofence(Geofence geofence)
        {
            if (!IsGeofenceRegistered(geofence.Id))
                GeofenceMonitor.Geofences.Add(geofence);
            else
                ReplaceGeofence(geofence.Id, geofence);
        }

        public void RemoveGeofence(Geofence geofence) => RemoveGeofence(geofence.Id);

        public void RemoveGeofence(string id)
        {
            if (!IsGeofenceRegistered(id))
                return;
            var toRemove = ReadGeofence(id);
            GeofenceMonitor.Geofences.Remove(toRemove);
        }

        public void ReplaceGeofence(string id, Geofence geofence)
        {
            var searchResult = GeofenceMonitor.Geofences.FirstOrDefault(searchedGeofence => searchedGeofence.Id == id);
            if (searchResult == null)
                GeofenceMonitor.Geofences.Add(geofence);
            else
            {
                GeofenceMonitor.Geofences.Remove(searchResult);
                GeofenceMonitor.Geofences.Add(geofence);
            }
        }
    }
}