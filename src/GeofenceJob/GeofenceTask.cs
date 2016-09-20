using BackgroundTask.Toast;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Extensions;
using CoreLibrary.Logger;
using CoreLibrary.Service;
using CoreLibrary.Service.Geofencing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Storage;
using Windows.UI.Notifications;

namespace BackgroundTask
{
    public sealed class GeofenceTask : IBackgroundTask
    {
        private readonly IRepository<Alarm> _alarmsRepository;
        private readonly IGeofenceService _geofenceService;
        private readonly ToastNotifier _toastNotifier;
        private HashSet<Alarm> _alarmsToUpdate;
        private ILogger _logger;

        private static ApplicationDataContainer Settings => ApplicationData.Current.LocalSettings;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceService = new GeofenceService();
            _alarmsRepository = new GenericRepository<Alarm>();
            _logger = new DatabseLogger(new GenericRepository<Log>());
            _alarmsToUpdate = new HashSet<Alarm>();
        }

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            try
            {
                var reports = _geofenceService.GeofenceStateChangeReports.ToList();

                var allAlarms = _alarmsRepository.GetAll().ToList();

                var triggeredAlarms = GetTriggeredAlarms(reports, allAlarms);

                var qualifiedToNotification = AlarmsQualifiedToNotification(reports, allAlarms);

                var notificationService = new AlarmsNotificationService(_toastNotifier, qualifiedToNotification);

                notificationService.Notify();

                ChangeFiredState(triggeredAlarms);

                DisableAlarms(qualifiedToNotification.Select(alarm => alarm.Alarm).ToList());

                UpdateAlarms();

                //RefreshGeofences(triggeredAlarms);
            }
            catch (Exception exception)
            {
                _logger.LogException($"Exception from {nameof(GeofenceTask)}", exception);
            }
        }

        private IList<TriggeredAlarm> AlarmsQualifiedToNotification(IList<GeofenceStateChangeReport> reports, IList<Alarm> alarms)
        {
            var activeAlarms = FindActiveAlarms(alarms);

            return GetTriggeredAlarms(reports, activeAlarms)
                .Where(alarm => !alarm.Alarm.Fired && alarm.Report.NewState == GeofenceState.Entered)
                .Distinct(new TriggeredAlarmEqualityComparer())
                .ToList();
        }

        private void ChangeFiredState(IList<TriggeredAlarm> triggeredAlarms)
        {
            triggeredAlarms
                .Where(alarm => alarm.Report.NewState == GeofenceState.Entered && !alarm.Alarm.Fired)
                .ForEach(alarm =>
                {
                    alarm.Alarm.Fired = true;
                    _alarmsToUpdate.Add(alarm.Alarm);
                });

            triggeredAlarms
                .Where(alarm => alarm.Report.NewState == GeofenceState.Exited && alarm.Alarm.Fired)
                .ForEach(alarm =>
                {
                    alarm.Alarm.Fired = false;
                    _alarmsToUpdate.Add(alarm.Alarm);
                });
        }

        private void DisableAlarms(IList<Alarm> alarms)
        {
            var alarmsToDisable = alarms.Where(alarm => string.IsNullOrEmpty(alarm.ActiveDays)).ToArray();

            if (!alarmsToDisable.Any())
                return;

            foreach (var alarm in alarmsToDisable)
            {
                alarm.IsActive = false;
                _alarmsToUpdate.Add(alarm);
            }
        }

        private IList<Alarm> FindActiveAlarms(IList<Alarm> allAlarms) => allAlarms.Where(alarm => alarm.IsActive && IsActiveToday(alarm)).ToList();

        private IList<TriggeredAlarm> GetTriggeredAlarms(IList<GeofenceStateChangeReport> reports, IList<Alarm> alarms)
        {
            var activeGeofences = reports
                .Where(report => report.NewState == GeofenceState.Entered || report.NewState == GeofenceState.Exited)
                .Join(alarms, report => report.Geofence.Id,
                      alarm => alarm.Name.ToString(),
                     (report, alarm) => new TriggeredAlarm(report, alarm))
                .ToList();

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

        private void RefreshGeofences(IList<TriggeredAlarm> triggeredAlarms)
        {
            var geofences = triggeredAlarms
                .Where(alarm => !string.IsNullOrEmpty(alarm.Alarm.ActiveDays))
                .Select(alarm => alarm.Report.Geofence);

            foreach (var geofence in geofences)
                _geofenceService.ReplaceGeofence(geofence.Id, geofence);
        }

        private void UpdateAlarms()
        {
            if (!_alarmsToUpdate.Any())
                return;
            Settings.Values["WasUpdated"] = true;
            _alarmsRepository.UpdateAll(_alarmsToUpdate.ToList());
        }
    }
}