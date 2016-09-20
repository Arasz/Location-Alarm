using BackgroundTask.Notifications.ToastTemplate;
using CoreLibrary.Data.DataModel.PersistentModel;
using System.Collections.Generic;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace BackgroundTask.Toast
{
    /// <summary>
    /// </summary>
    internal sealed class AlarmsNotificationService : IAlarmNotificationService
    {
        private readonly ToastNotifier _toastNotifier;
        private readonly IEnumerable<TriggeredAlarm> _triggeredAlarms;

        public AlarmsNotificationService(ToastNotifier toastNotifier, IEnumerable<TriggeredAlarm> triggeredAlarms)
        {
            _toastNotifier = toastNotifier;
            _triggeredAlarms = triggeredAlarms;
        }

        public void Notify()
        {
            foreach (var triggeredAlarm in _triggeredAlarms)
                _toastNotifier.Show(MakeToast(triggeredAlarm.Report, triggeredAlarm.Alarm));
        }

        private ToastNotification MakeToast(GeofenceStateChangeReport report, Alarm alarm)
        {
            var tost = new SimpleToast("You have entered location", alarm.Name, alarm.AlarmSound);

            return new ToastNotification(tost.ToastBlueprint);
        }
    }
}