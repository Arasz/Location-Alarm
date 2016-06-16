using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmModel
    {
        private ObservableCollection<LocationAlarm> _alarms = new ObservableCollection<LocationAlarm>();

        public INotifyCollectionChanged Collection => _alarms;

        public LocationAlarmModel()
        {
        }

        public void Add(LocationAlarm locationAlarm)
        {
            _alarms.Add(locationAlarm);
        }

        public LocationAlarm CreateTransitive()
        {
            var alarm = new LocationAlarm();
            return alarm;
        }

        public void Remove(LocationAlarm locationAlarm)
        {
            _alarms.Remove(locationAlarm);
        }
    }
}