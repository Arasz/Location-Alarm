﻿using CoreLibrary.DataModel;
using CoreLibrary.Service;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmModel
    {
        private readonly ObservableCollection<GeolocationAlarm> _alarms;

        private readonly IGeofenceService _geofenceService;

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
            _geofenceService.RegisterGeofence(alarm.Geofence);
        }

        public void Update(GeolocationAlarm alarm)
        {
            _repository.Update(alarm);

            _geofenceService.ReplaceGeofence(alarm.Name, alarm.Geofence);
        }
    }
}