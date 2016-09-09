using CoreLibrary.DataModel;
using CoreLibrary.Service;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace LocationAlarm.Model
{
    public class LocationAlarmModel
    {
        private readonly IGeofenceService _geofenceService;
        private readonly GelocationAlarmRepository _repository;
        private ObservableCollection<GeolocationAlarm> _alarms;
        public INotifyCollectionChanged GeolocationAlarms => _alarms;

        public GeolocationAlarm NewAlarm => new GeolocationAlarm();

        public LocationAlarmModel(GelocationAlarmRepository repository, IGeofenceService geofenceService)
        {
            _repository = repository;
            _geofenceService = geofenceService;
            _alarms = new ObservableCollection<GeolocationAlarm>();
        }

        public async Task DeleteAsync(GeolocationAlarm alarm)
        {
            _alarms.Remove(alarm);
            await _repository.DeleteAsync(alarm).ConfigureAwait(false);
            _geofenceService.RemoveGeofence(alarm.Name);
        }

        public async Task SaveAsync(GeolocationAlarm alarm)
        {
            _alarms.Add(alarm);
            await _repository.CreateAsync(alarm).ConfigureAwait(false);
            _geofenceService.RegisterGeofence(alarm.Geofence);
        }

        public async Task Update(GeolocationAlarm alarm)
        {
            await _repository.Update(alarm).ConfigureAwait(false);

            _geofenceService.ReplaceGeofence(alarm.Name, alarm.Geofence);
        }
    }
}