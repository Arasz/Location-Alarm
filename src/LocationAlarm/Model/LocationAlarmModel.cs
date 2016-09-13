﻿using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.DataModel;
using CoreLibrary.Service;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Application model 
    /// </summary>
    public class LocationAlarmModel
    {
        private readonly IGeofenceService _geofenceService;

        private readonly IRepository<GeolocationAlarm> _repository;

        public ObservableCollection<GeolocationAlarm> GeolocationAlarms { get; private set; }

        public GeolocationAlarm NewAlarm => new GeolocationAlarm();

        public LocationAlarmModel(IRepository<GeolocationAlarm> repository, IGeofenceService geofenceService)
        {
            _repository = repository;
            _geofenceService = geofenceService;
            GeolocationAlarms = new ObservableCollection<GeolocationAlarm>();
        }

        public async Task DeleteAsync(GeolocationAlarm alarm)
        {
            GeolocationAlarms.Remove(alarm);
            await _repository.DeleteAsync(alarm).ConfigureAwait(false);
            _geofenceService.RemoveGeofence(alarm.Name);
        }

        public async Task ReloadDataAsync()
        {
            GeolocationAlarms.Clear();
            var savedAlarms = await _repository.GetAllAsync().ConfigureAwait(true);
            foreach (var geolocationAlarm in savedAlarms)
                GeolocationAlarms.Add(geolocationAlarm);
        }

        public async Task SaveAsync(GeolocationAlarm alarm)
        {
            GeolocationAlarms.Add(alarm);
            await _repository.InsertAsync(alarm).ConfigureAwait(false);
            _geofenceService.RegisterGeofence(alarm.Geofence);
        }

        public async Task ToggleAlarmAsync(GeolocationAlarm alarm)
        {
            await _repository.UpdateAsync(alarm).ConfigureAwait(false);

            if (alarm.IsActive)
                _geofenceService.RegisterGeofence(alarm.Geofence);
            else
                _geofenceService.RemoveGeofence(alarm.Geofence);
        }

        public async Task UpdateAsync(GeolocationAlarm alarm)
        {
            if (!GeolocationAlarms.Contains(alarm))
                return;

            await _repository.UpdateAsync(alarm).ConfigureAwait(false);
            _geofenceService.ReplaceGeofence(alarm.Name, alarm.Geofence);
        }
    }
}