using System.Collections.Generic;
using Windows.Devices.Geolocation.Geofencing;

namespace CoreLibrary.Service
{
    /// <summary>
    /// Can perform CRUD operations with geofence class 
    /// </summary>
    public interface IGeofenceService
    {
        /// <summary>
        /// Collection of all active geofences 
        /// </summary>
        IEnumerable<Geofence> AllActiveGeofences { get; }

        /// <summary>
        /// Reports of geofence state change 
        /// </summary>
        IReadOnlyList<GeofenceStateChangeReport> GeofenceStateChangeReports { get; }

        /// <summary>
        /// Monitor status 
        /// </summary>
        GeofenceMonitorStatus Status { get; }

        /// <summary>
        /// Check if geofence with given id is registered 
        /// </summary>
        bool IsGeofenceRegistered(string id);

        /// <summary>
        /// Read geofence of given id 
        /// </summary>
        Geofence ReadGeofence(string id);

        /// <summary>
        /// Register geofence 
        /// </summary>
        /// <param name="geofence"></param>
        void RegisterGeofence(Geofence geofence);

        /// <summary>
        /// Remove geofence 
        /// </summary>
        /// <param name="geofence"></param>
        void RemoveGeofence(Geofence geofence);

        /// <summary>
        /// Remove geofence with given id 
        /// </summary>
        /// <param name="geofence"></param>
        void RemoveGeofence(string id);

        /// <summary>
        /// Replace geofence of given id with new geofence. If geofence of given id doesn't exist,
        /// register it
        /// </summary>
        /// <param name="id"></param>
        /// <param name="geofence"></param>
        void ReplaceGeofence(string id, Geofence geofence);
    }
}