using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Model
{
    public class LocationAlarmManager
    {
        private ObservableCollection<AlarmModel> _alarms = new ObservableCollection<AlarmModel>();

        public INotifyCollectionChanged Collection => _alarms;

        public LocationAlarmManager()
        {
        }

        public void Add(AlarmModel alarm)
        {
            _alarms.Add(alarm);
        }

        public AlarmModel CreateTransitive()
        {
            var alarm = new AlarmModel();
            return alarm;
        }

        public void Remove(AlarmModel alarmModel)
        {
            _alarms.Remove(alarmModel);
        }
    }
}