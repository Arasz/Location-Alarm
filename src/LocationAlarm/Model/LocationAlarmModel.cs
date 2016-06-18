using CoreLibrary.DataModel;
using CoreLibrary.Service;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Devices.Geolocation.Geofencing;

namespace LocationAlarm.Model
{
    public class LocationAlarmModel
    {
        private readonly ObservableCollection<GeolocationAlarm> _alarms;

        private readonly IGeofenceService _geofenceService;

        private readonly Dictionary<string, Geofence> _registeredGeofences = new Dictionary<string, Geofence>();

        private readonly GelocationAlarmRepository _repository;

        public INotifyCollectionChanged GeolocationAlarms => _alarms;

        public GeolocationAlarm NewAlarm => new GeolocationAlarm();

        public LocationAlarmModel(GelocationAlarmRepository repository, IGeofenceService geofenceService)
        {
            _repository = repository;
            _geofenceService = geofenceService;
            _alarms = new ObservableCollection<GeolocationAlarm>();
        }

        public void Delete(GeolocationAlarm alarm)
        {
            _alarms.Remove(alarm);
            _repository.Delete(alarm);
            _geofenceService.RemoveGeofence(alarm.Name);
        }

        public void Save(GeolocationAlarm alarm)
        {
            _alarms.Add(alarm);
            alarm.Id = _repository.Create(alarm);
            var geofence = CreateGeofenceFromAlarm(alarm);
            _geofenceService.RegisterGeofence(geofence);
        }

        public void Update(GeolocationAlarm alarm)
        {
            _repository.Update(alarm);

            _geofenceService.ReplaceGeofence(alarm.Name, CreateGeofenceFromAlarm(alarm));
        }

        private Geofence CreateGeofenceFromAlarm(GeolocationAlarm alarm)
        {
            _registeredGeofences[alarm.Name] = new GeofenceBuilder()
                .WithDefaultParameters()
                .SetRequiredId(alarm.Name)
                .ThenSetGeocircle(alarm.Geoposition, alarm.Radius)
                .IsUsedOnce(!alarm.ActiveDays.Any())
                .Build();
            return _registeredGeofences[alarm.Name];
        }
    }
}