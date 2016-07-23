using CoreLibrary.Service;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class GeofenceTask : IGeofenceTask
    {
        private GeofenceMonitor _geofenceMonitor;
        private IGeofenceService _geofenceService;
        private ToastNotifier _toastNotifier;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceMonitor = GeofenceMonitor.Current;
        }

        public void Run(IBackgroundTaskInstance instance)
        {
            var reports = _geofenceMonitor.ReadReports();

            var selectedReports = FilterReports(reports);

            _toastNotifier.Show(CreateToast(selectedReports));
        }

        private ToastNotification CreateToast(IEnumerable<GeofenceStateChangeReport> selectedReports)
        {
            var selectedReport = selectedReports.First();

            var toastContent = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            var TextNodes = toastContent.GetElementsByTagName("text");

            if (selectedReport.NewState == GeofenceState.Entered)
            {
                TextNodes[0].AppendChild(toastContent.CreateTextNode("You are pretty close to your room"));
                TextNodes[1].AppendChild(toastContent.CreateTextNode(selectedReport.Geofence.Id));
            }
            else if (selectedReport.NewState == GeofenceState.Exited)
            {
                TextNodes[0].AppendChild(toastContent.CreateTextNode("You are pretty close to your room"));
                TextNodes[1].AppendChild(toastContent.CreateTextNode(selectedReport.Geofence.Id));
            }

            var settings = ApplicationData.Current.LocalSettings;

            if (settings.Values.ContainsKey("Status"))
            {
                settings.Values["Status"] = selectedReport.NewState;
            }
            else
            {
                settings.Values.Add(new KeyValuePair<string, object>("Status", selectedReport.NewState.ToString()));
            }

            return new ToastNotification(toastContent);
        }

        private IEnumerable<GeofenceStateChangeReport> FilterReports(IEnumerable<GeofenceStateChangeReport> reports)
        {
            return null;
        }
    }
}