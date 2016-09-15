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
            var toastContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var textNodes = toastContent.GetElementsByTagName("text");

            textNodes[0].AppendChild(toastContent.CreateTextNode("You have entered location "));
            textNodes[1].AppendChild(toastContent.CreateTextNode(alarm.Name));

            return new ToastNotification(toastContent);
        }
    }
}