using LocationAlarm.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmsManager
    {
        private ObservableCollection<LocationAlarm> _alarms;
        private LocationAlarmRepository _repository;
        public INotifyCollectionChanged Collection => _alarms;

        public LocationAlarmsManager()
        {
            _repository = new LocationAlarmRepository(new DataContext());
            _alarms = new ObservableCollection<LocationAlarm>(_repository.ReadAllLocationAlarms());
        }

        public void Add(LocationAlarm locationAlarm)
        {
            locationAlarm.Id = _repository.InsertLocationAlarm(locationAlarm);
            _alarms.Add(locationAlarm);
        }

        public LocationAlarm CreateTransitive()
        {
            var alarm = new LocationAlarm();
            return alarm;
        }

        public void Remove(LocationAlarm locationAlarm)
        {
            _repository.DeleteLocationAlarm(locationAlarm);
            _alarms.Remove(locationAlarm);
        }
    }
}