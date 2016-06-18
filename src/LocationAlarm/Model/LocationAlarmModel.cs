using CoreLibrary.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmModel
    {
        private ObservableCollection<GeolocationAlarm> _alarms;
        private GelocationAlarmRepository _repository;

        public INotifyCollectionChanged GeolocationAlarms => _alarms;

        public GeolocationAlarm NewAlarm => new GeolocationAlarm();

        public LocationAlarmModel(GelocationAlarmRepository repository)
        {
            _repository = repository;
            _alarms = new ObservableCollection<GeolocationAlarm>();
        }

        public void Delete(GeolocationAlarm alarm)
        {
            _alarms.Remove(alarm);
            _repository.Delete(alarm);
        }

        public void Save(GeolocationAlarm alarm)
        {
            _alarms.Add(alarm);
            alarm.Id = _repository.Create(alarm);
        }
    }
}