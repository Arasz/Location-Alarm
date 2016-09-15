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
    public sealed class GeofenceTask : IGeofenceTask
    {
        private IRepository<Alarm> _alarmsRepository;
        private IGeofenceService _geofenceService;
        private ToastNotifier _toastNotifier;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceService = new GeofenceService();
            _alarmsRepository = new GenericRepository<Alarm>();
        }

        public async void Run(IBackgroundTaskInstance instance)
        {
            var reports = _geofenceService.GeofenceStateChangeReports;

            var alarms = await FindActiveAlarmsAsync().ConfigureAwait(false);

            var triggeredAlarms = GetTriggeredAlarmsAsync(reports, alarms).ToList();

            var notificationService = new AlarmsNotificationService(_toastNotifier, triggeredAlarms);

            notificationService.Notify();

            await ChangeAlarmsStateAsync(triggeredAlarms.Select(tuple => tuple.Item2)).ConfigureAwait(false);
        }

        private async Task ChangeAlarmsStateAsync(IEnumerable<Alarm> alarms)
        {
            var alarmsToDisable = alarms.Where(alarm => string.IsNullOrEmpty(alarm.ActiveDays));
            await _alarmsRepository.UpdateAllAsync(alarmsToDisable).ConfigureAwait(false);

            foreach (var alarm in alarmsToDisable)
                _geofenceService.RemoveGeofence(alarm.Name);
        }

        private async Task<IEnumerable<Alarm>> FindActiveAlarmsAsync()
        {
            var activeAlarms = await _alarmsRepository.FindAsync(alarm => alarm.IsActive).ConfigureAwait(false);
            return activeAlarms.Where(IsActiveToday);
        }

        private IEnumerable<Tuple<GeofenceStateChangeReport, Alarm>> GetTriggeredAlarmsAsync(IEnumerable<GeofenceStateChangeReport> reports, IEnumerable<Alarm> alarms)
        {
            var activeGeofences = reports
                .Where(report => report.NewState == GeofenceState.Entered)
                .Join(alarms, report => report.Geofence.Id,
                      alarm => alarm.Name,
                     (report, alarm) => new Tuple<GeofenceStateChangeReport, Alarm>(report, alarm));

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