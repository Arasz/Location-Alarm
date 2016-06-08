using ArrivalAlarm.Model;
using LocationAlarm.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Devices.Geolocation;

namespace LocationAlarm.Repository
{
    public class AlarmsRepository
    {
        private ObservableCollection<AlarmModel> _alarms = new ObservableCollection<AlarmModel>();

        public INotifyCollectionChanged Collection => _alarms;

        public AlarmsRepository()
        {
            _alarms.Add(new AlarmModel(new MonitoredArea("Poznan", new GeofenceBuilder().SetRequiredId("P1").ThenSetGeocircle(new BasicGeoposition(), 4d)))
            {
                Label = "Alarm praca",
                ActiveDays = new HashSet<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday },
                IsActive = true,
                IsCyclic = true,
            });
        }

        public void Add(AlarmModel alarm)
        {
            _alarms.Add(alarm);
        }

        public AlarmModel Create()
        {
            var alarm = new AlarmModel();
            Add(alarm);
            return alarm;
        }

        public void Remove(AlarmModel alarm)
        {
            _alarms.Remove(alarm);
        }

        public AlarmModel Retrieve(int index) => _alarms.ElementAt(index);

        public void Update(AlarmModel alarm)
        {
            Remove(alarm);
            _alarms.Add(alarm);
        }
    }
}