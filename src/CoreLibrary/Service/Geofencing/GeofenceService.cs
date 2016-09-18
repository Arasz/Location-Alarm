using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation.Geofencing;

namespace CoreLibrary.Service.Geofencing
{
    public class GeofenceService : IGeofenceService
    {
        private readonly GeofenceMonitor _geofenceMonitor;

        public IEnumerable<Geofence> AllActiveGeofences => _geofenceMonitor.Geofences.ToList();

        public IReadOnlyList<GeofenceStateChangeReport> GeofenceStateChangeReports => _geofenceMonitor.ReadReports().ToList();

        public GeofenceMonitorStatus Status => _geofenceMonitor.Status;

        public GeofenceService()
        {
            _geofenceMonitor = GeofenceMonitor.Current;
            _geofenceMonitor.StatusChanged += (sender, args) =>
            {
                var status = Status;
            };
        }

        public bool IsGeofenceRegistered(string id) => _geofenceMonitor.Geofences.Any(geofence => geofence.Id == id);

        public Geofence ReadGeofence(string id) => _geofenceMonitor.Geofences.FirstOrDefault(geofence => geofence.Id == id);

        public void RegisterGeofence(Geofence geofence)
        {
            if (!IsGeofenceRegistered(geofence.Id))
                _geofenceMonitor.Geofences.Add(geofence);
            else
                ReplaceGeofence(geofence.Id, geofence);
        }

        public void RemoveGeofence(Geofence geofence) => RemoveGeofence(geofence.Id);

        public void RemoveGeofence(string id)
        {
            if (!IsGeofenceRegistered(id))
                return;
            var toRemove = ReadGeofence(id);
            _geofenceMonitor.Geofences.Remove(toRemove);
        }

        public void ReplaceGeofence(string id, Geofence geofence)
        {
            var searchResult = _geofenceMonitor.Geofences.FirstOrDefault(searchedGeofence => searchedGeofence.Id == id);
            if (searchResult == null)
                _geofenceMonitor.Geofences.Add(geofence);
            else
            {
                _geofenceMonitor.Geofences.Remove(searchResult);
                _geofenceMonitor.Geofences.Add(geofence);
            }
        }
    }
}