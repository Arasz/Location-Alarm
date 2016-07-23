﻿using System;
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

            _geofenceMonitor.GeofenceStateChanged += GeofenceMonitorOnGeofenceStateChanged;
            _geofenceMonitor.StatusChanged += GeofenceMonitorOnStatusChanged;
        }

        public bool IsGeofenceRegistered(string id) => _geofenceMonitor.Geofences.Any(geofence => geofence.Id == id);

        public Geofence ReadGeofence(string id) => _geofenceMonitor.Geofences.FirstOrDefault(geofence => geofence.Id == id);

        public void RegisterGeofence(Geofence geofence) => _geofenceMonitor.Geofences.Add(geofence);

        public void RemoveGeofence(Geofence geofence) => _geofenceMonitor.Geofences.Remove(geofence);

        public void RemoveGeofence(string id)
        {
            var geofence = ReadGeofence(id);
            if (geofence != null) RemoveGeofence(geofence);

            throw new ArgumentException($"Geofence of {id} id isn't registered");
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

        private void GeofenceMonitorOnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
        }

        private void GeofenceMonitorOnStatusChanged(GeofenceMonitor sender, object args)
        {
        }
    }
}