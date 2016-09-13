using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation.Geofencing;

namespace CoreLibrary.Service.Geofencing
{
    public class GeofenceService : IGeofenceService
    {
        private readonly GeofenceMonitor _geofenceMonitor;

        public IEnumerable<Geofence> AllActiveGeofences => _geofenceMonitor.Geofences.AsEnumerable();

        public IReadOnlyList<GeofenceStateChangeReport> GeofenceStateChangeReports => _geofenceMonitor.ReadReports();

        public GeofenceService()
        {
            _geofenceMonitor = GeofenceMonitor.Current;
        }

        public bool IsGeofenceRegistered(string id) => _geofenceMonitor.Geofences.Any(geofence => geofence.Id == id);

        public Geofence ReadGeofence(string id) => _geofenceMonitor.Geofences.FirstOrDefault(geofence => geofence.Id == id);

        public void RegisterGeofence(Geofence geofence)
        {
            if (!IsGeofenceRegistered(geofence.Id))
                _geofenceMonitor.Geofences.Add(geofence);
        }

        public void RemoveGeofence(Geofence geofence)
        {
            if (!IsGeofenceRegistered(geofence.Id))
                return;
            var toRemove = ReadGeofence(geofence.Id);
            _geofenceMonitor.Geofences.Remove(toRemove);
        }

        public void RemoveGeofence(string id)
        {
            var geofence = ReadGeofence(id);
            if (geofence == null)
                throw new ArgumentException($"Geofence of {id} id isn't registered");
            RemoveGeofence(geofence);
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