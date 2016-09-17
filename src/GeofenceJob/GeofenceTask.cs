using BackgroundTask.Toast;
using CoreLibrary.Data;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Service;
using CoreLibrary.Service.Geofencing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class GeofenceTask : IBackgroundTask
    {
        private IRepository<Alarm> _alarmsRepository;
        private BackgroundTaskDeferral _deferral;
        private IGeofenceService _geofenceService;
        private ToastNotifier _toastNotifier;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceService = new GeofenceService();
            _alarmsRepository = new GenericRepository<Alarm>();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            taskInstance.Progress = 0;

            var reports = _geofenceService.GeofenceStateChangeReports;

            taskInstance.Progress = 10;

            var alarms = await FindActiveAlarmsAsync().ConfigureAwait(false);

            taskInstance.Progress = 30;

            var triggeredAlarms = GetTriggeredAlarmsAsync(reports, alarms).ToList();

            taskInstance.Progress = 60;

            var notificationService = new AlarmsNotificationService(_toastNotifier, triggeredAlarms);

            taskInstance.Progress = 70;

            notificationService.Notify();

            taskInstance.Progress = 80;

            await ChangeAlarmsStateAsync(triggeredAlarms.Select(triggeredAlarm => triggeredAlarm.Alarm)).ConfigureAwait(false);

            taskInstance.Progress = 100;
            _deferral.Complete();
        }

        private async Task ChangeAlarmsStateAsync(IEnumerable<Alarm> alarms)
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
                      alarm => alarm.Name,
                     (report, alarm) => new TriggeredAlarm(report, alarm));

            return activeGeofences;
        }

        private bool IsActiveToday(Alarm alarm)
        {
            var activeDays = alarm.ActiveDays;
            //One time alarm (shot and delete)
            if (string.IsNullOrEmpty(activeDays))
                return true;

            var parsedDays = activeDays.Split(',').Select(dayName => new WeekDay(dayName).Day);
            return parsedDays.Contains(DateTime.Today.DayOfWeek);
        }
    }
}