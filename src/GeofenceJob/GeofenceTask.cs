using BackgroundTask.Toast;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Service;
using CoreLibrary.Service.Geofencing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class GeofenceTask : IBackgroundTask
    {
        private readonly IRepository<Alarm> _alarmsRepository;
        private readonly IGeofenceService _geofenceService;
        private readonly ToastNotifier _toastNotifier;
        private BackgroundTaskDeferral _deferral;
        private IBackgroundTaskInstance _taskInstance;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceService = new GeofenceService();
            _alarmsRepository = new GenericRepository<Alarm>();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _taskInstance = taskInstance;
            _deferral = _taskInstance.GetDeferral();

            var reports = _geofenceService.GeofenceStateChangeReports;

            var alarms = await FindActiveAlarmsAsync().ConfigureAwait(false);

            var triggeredAlarms = GetTriggeredAlarmsAsync(reports, alarms).ToList();

            var notificationService = new AlarmsNotificationService(_toastNotifier, triggeredAlarms);

            notificationService.Notify();

            await DisableAlarmsAsync(triggeredAlarms.Select(triggeredAlarm => triggeredAlarm.Alarm)).ConfigureAwait(false);

            await ReregisterGeofences(triggeredAlarms).ConfigureAwait(false);

            _deferral.Complete();
        }

        private async Task DisableAlarmsAsync(IEnumerable<Alarm> alarms)
        {
            var alarmsToDisable = alarms.Where(alarm => string.IsNullOrEmpty(alarm.ActiveDays)).ToArray();

            if (!alarmsToDisable.Any())
                return;

            foreach (var alarm in alarmsToDisable)
                alarm.IsActive = false;

            await _alarmsRepository.UpdateAllAsync(alarmsToDisable).ConfigureAwait(false);
        }

        private async Task<IEnumerable<Alarm>> FindActiveAlarmsAsync()
        {
            var activeAlarms = await _alarmsRepository.FindAsync(alarm => alarm.IsActive).ConfigureAwait(false);
            return activeAlarms.Where(IsActiveToday);
        }

        private IEnumerable<TriggeredAlarm> GetTriggeredAlarmsAsync(IEnumerable<GeofenceStateChangeReport> reports, IEnumerable<Alarm> alarms)
        {
            var activeGeofences = reports
                .Where(report => report.NewState == GeofenceState.Entered)
                .Join(alarms, report => report.Geofence.Id,
                      alarm => alarm.Name.ToString(),
                     (report, alarm) => new TriggeredAlarm(report, alarm));

            return activeGeofences;
        }

        private bool IsActiveToday(Alarm alarm)
        {
            var activeDays = alarm.ActiveDays;
            //One time alarm (shot and delete)
            if (string.IsNullOrEmpty(activeDays))
                return true;

            var parsedDays = activeDays.Split(',');
            return parsedDays.Contains(DateTimeFormatInfo.CurrentInfo.GetDayName(DateTime.Today.DayOfWeek));
        }

        private async Task ReregisterGeofences(List<TriggeredAlarm> triggeredAlarms)
        {
            var geofences = triggeredAlarms
                .Where(alarm => !string.IsNullOrEmpty(alarm.Alarm.ActiveDays))
                .Select(alarm => alarm.Report.Geofence);

            foreach (var geofence in geofences)
                _geofenceService.ReplaceGeofence(geofence.Id, geofence);
        }
    }
}