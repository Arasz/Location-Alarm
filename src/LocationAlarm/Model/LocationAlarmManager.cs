using CoreLibrary.DataModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmManager
    {
        private ObservableCollection<GeolocationAlarm> _alarms = new ObservableCollection<GeolocationAlarm>();

        public INotifyCollectionChanged Collection => _alarms;

        public LocationAlarmManager()
        {
        }

        public void Add(GeolocationAlarm alarm)
        {
            _alarms.Add(alarm);
        }

        public GeolocationAlarm CreateTransitive()
        {
            var alarm = new GeolocationAlarm();
            return alarm;
        }

        public void Remove(GeolocationAlarm alarmModel)
        {
            _alarms.Remove(alarmModel);
        }
    }
}