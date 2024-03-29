﻿using BackgroundTask;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Geofencing;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Service;
using GalaSoft.MvvmLight.Threading;
using LocationAlarm.BackgroundTask;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Application model 
    /// </summary>
    public class LocationAlarmModel
    {
        private readonly BackgroundTaskManager<GeofenceTask> _backgroundTaskManager;
        private readonly GeofenceBuilder _builder;
        private readonly IGeofenceService _geofenceService;
        private readonly IRepository<Alarm> _repository;

        private readonly object syncRoot = new object();

        private volatile bool _isDataReloading;

        public ObservableCollection<Alarm> GeolocationAlarms { get; }

        private static ApplicationDataContainer Settings
        {
            get { return ApplicationData.Current.LocalSettings; }
        }

        public LocationAlarmModel(IRepository<Alarm> repository, IGeofenceService geofenceService, BackgroundTaskManager<GeofenceTask> backgroundTaskManager, GeofenceBuilder builder)
        {
            _repository = repository;
            _geofenceService = geofenceService;
            _backgroundTaskManager = backgroundTaskManager;
            _builder = builder;
            _backgroundTaskManager.TaskCompleted += BackgroundTaskManagerOnTaskCompleted;
            GeolocationAlarms = new ObservableCollection<Alarm>();
        }

        public async Task DeleteAsync(Alarm alarm)
        {
            GeolocationAlarms.Remove(alarm);
            await _repository.DeleteAsync(alarm).ConfigureAwait(false);
            _geofenceService.RemoveGeofence(alarm.Name);
        }

        public async Task ReloadDataAsync()
        {
            lock (syncRoot)
            {
                if (_isDataReloading)
                    return;
                _isDataReloading = true;
            }

            var savedAlarms = await _repository.GetAllAsync().ConfigureAwait(true);

            GeolocationAlarms.Clear();
            foreach (var geolocationAlarm in savedAlarms)
            {
                GeolocationAlarms.Add(geolocationAlarm);
                if (geolocationAlarm.IsActive && !_geofenceService.IsGeofenceRegistered(geolocationAlarm.Name))
                    _geofenceService.RegisterGeofence(_builder.BuildFromAlarm(geolocationAlarm));
            }

            lock (syncRoot)
                _isDataReloading = false;
        }

        public async Task SaveAsync(Alarm alarm)
        {
            var notUniqueAlarm = GeolocationAlarms.FirstOrDefault(geolocationAlarm => geolocationAlarm.Name == alarm.Name);
            if (notUniqueAlarm == null)
                await InsertAsync(alarm).ConfigureAwait(false);
            else
            {
                ReplaceAlram(alarm, notUniqueAlarm);
                await UpdateAsync(alarm).ConfigureAwait(false);
            }
        }

        public async Task ToggleAlarmAsync(Alarm alarm)
        {
            if (alarm.IsActive)
                _geofenceService.RegisterGeofence(_builder.BuildFromAlarm(alarm));
            else
                _geofenceService.RemoveGeofence(alarm.Name);

            // alarm.Fired = false;

            await _repository.UpdateAsync(alarm).ConfigureAwait(false);
        }

        public async Task UpdateAsync(Alarm alarm)
        {
            if (!GeolocationAlarms.Contains(alarm))
                return;

            ReplaceAlarm(alarm);
            await _repository.UpdateAsync(alarm).ConfigureAwait(false);
            _geofenceService.ReplaceGeofence(alarm.Name, _builder.BuildFromAlarm(alarm));
        }

        private async void BackgroundTaskManagerOnTaskCompleted(object sender, EventArgs eventArgs)
        {
            if (Settings.Values.ContainsKey("WasUpdated") && (bool)Settings.Values["WasUpdated"])
            {
                await DispatcherHelper.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    async () => await ReloadDataAsync().ConfigureAwait(false));
            }
        }

        private async Task InsertAsync(Alarm alarm)
        {
            GeolocationAlarms.Add(alarm);
            await _repository.InsertAsync(alarm).ConfigureAwait(false);
            _geofenceService.RegisterGeofence(_builder.BuildFromAlarm(alarm));
        }

        private void ReplaceAlarm(Alarm replacement)
        {
            var replaced = GeolocationAlarms.First(alarm => alarm.Id == replacement.Id);
            ReplaceAlram(replacement, replaced);
        }

        private void ReplaceAlram(Alarm replacement, Alarm replaced)
        {
            replacement.Id = replaced.Id;
            var index = GeolocationAlarms.IndexOf(replaced);
            GeolocationAlarms.RemoveAt(index);
            GeolocationAlarms.Insert(index, replacement);
        }
    }
}