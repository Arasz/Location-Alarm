using LocationAlarm.Model;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace LocationAlarm.Repository
{
    public class AlarmsRepository
    {
        private ObservableCollection<AlarmModel> _alarms = new ObservableCollection<AlarmModel>();

        public INotifyCollectionChanged Collection => _alarms;

        public AlarmsRepository()
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

        public void Remove(AlarmModel alarm)
        {
            _alarms.Remove(alarm);
        }

        public void Update(AlarmModel alarm)
        {
            Remove(alarm);
            _alarms.Add(alarm);
        }
    }
}