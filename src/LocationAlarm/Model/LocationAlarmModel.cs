using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.DataModel;
using CoreLibrary.Service;
using GalaSoft.MvvmLight.Threading;
using LocationAlarm.Tasks;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Application model 
    /// </summary>
    public class LocationAlarmModel
    {
        private readonly BackgroundTaskManager _backgroundTaskManager;
        private readonly IGeofenceService _geofenceService;
        private readonly IRepository<GeolocationAlarm> _repository;

        public ObservableCollection<GeolocationAlarm> GeolocationAlarms { get; private set; }

        public GeolocationAlarm NewAlarm => new GeolocationAlarm();

        public LocationAlarmModel(IRepository<GeolocationAlarm> repository, IGeofenceService geofenceService, BackgroundTaskManager backgroundTaskManager)
        {
            _repository = repository;
            _geofenceService = geofenceService;
            _backgroundTaskManager = backgroundTaskManager;
            _backgroundTaskManager.TaskCompleted += BackgroundTaskManagerOnTaskCompleted;
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
            var notUniqueAlarm = GeolocationAlarms.FirstOrDefault(geolocationAlarm => geolocationAlarm.Name == alarm.Name);
            if (notUniqueAlarm == null)
                await InsertAsync(alarm).ConfigureAwait(false);
            else
            {
                ReplaceAlram(alarm, notUniqueAlarm);
                await UpdateAsync(alarm).ConfigureAwait(false);
            }
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

        private async void BackgroundTaskManagerOnTaskCompleted(object sender, EventArgs eventArgs)
        {
            await DispatcherHelper.UIDispatcher.RunAsync(CoreDispatcherPriority.Normal,
               async () => await ReloadDataAsync().ConfigureAwait(false));
        }

        private async Task InsertAsync(GeolocationAlarm alarm)
        {
            GeolocationAlarms.Add(alarm);
            await _repository.InsertAsync(alarm).ConfigureAwait(false);
            _geofenceService.RegisterGeofence(alarm.Geofence);
        }

        private void ReplaceAlram(GeolocationAlarm replacement, GeolocationAlarm replaced)
        {
            replacement.Id = replaced.Id;
            var index = GeolocationAlarms.IndexOf(replaced);
            GeolocationAlarms.RemoveAt(index);
            GeolocationAlarms.Insert(index, replacement);
        }
    }
}