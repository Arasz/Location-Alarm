using CoreLibrary.Data.DataModel.PersistentModel;
using Windows.Devices.Geolocation.Geofencing;

namespace BackgroundTask
{
    internal class TriggeredAlarm
    {
        public Alarm Alarm { get; }

        public GeofenceStateChangeReport Report { get; }

        public TriggeredAlarm(GeofenceStateChangeReport report, Alarm alarm)
        {
            Report = report;
            Alarm = alarm;
        }
    }
}