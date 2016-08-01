using CoreLibrary.DataModel;
using CoreLibrary.Service;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
            _alarms = new ObservableCollection<GeolocationAlarm>(_repository.ReadAll());
        }

        public void Delete(GeolocationAlarm alarm)
        {
            _alarms.Remove(alarm);
            _repository.Delete(alarm);
            _geofenceService.RemoveGeofence(alarm.Name);
        }

        public void SaveAsync(GeolocationAlarm alarm)
        {
            _alarms.Add(alarm);
            _repository.Insert(alarm);
            _geofenceService.RegisterGeofence(alarm.Geofence);
        }

        public void Update(GeolocationAlarm alarm)
        {
            _repository.Update(alarm);

            _geofenceService.ReplaceGeofence(alarm.Name, alarm.Geofence);
        }
    }
}