﻿using BackgroundTask.Toast;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Logger;
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
        private HashSet<Alarm> _alarmsToUpdate;
        private BackgroundTaskDeferral _deferral;
        private ILogger _logger;
        private IBackgroundTaskInstance _taskInstance;

        public GeofenceTask()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
            _geofenceService = new GeofenceService();
            _alarmsRepository = new GenericRepository<Alarm>();
            _logger = new DatabseLogger(new GenericRepository<Log>());
            _alarmsToUpdate = new HashSet<Alarm>();
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _taskInstance = taskInstance;
            _deferral = _taskInstance.GetDeferral();

            try
            {
                var reports = _geofenceService.GeofenceStateChangeReports.ToList();

                var alarms = await FindActiveAlarmsAsync().ConfigureAwait(false);

                var triggeredAlarms = GetTriggeredAlarmsAsync(reports, alarms);

                var qualifiedToNotification = AlarmsQualifiedToNotification(triggeredAlarms);

                var notificationService = new AlarmsNotificationService(_toastNotifier, qualifiedToNotification);

                notificationService.Notify();

                ChangeFiredState(triggeredAlarms);

                DisableAlarms(triggeredAlarms.Select(triggeredAlarm => triggeredAlarm.Alarm).ToList());

                await UpdateAlarms().ConfigureAwait(false);

                await ReregisterGeofences(triggeredAlarms).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await _logger.LogExceptionAsync($"Exception from {nameof(GeofenceTask)}", exception).ConfigureAwait(false);
            }

            _deferral.Complete();
        }

        private static List<TriggeredAlarm> AlarmsQualifiedToNotification(IList<TriggeredAlarm> triggeredAlarms)
            => triggeredAlarms.Where(alarm => !alarm.Alarm.Fired && alarm.Report.NewState == GeofenceState.Entered).ToList();

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

        private async Task<IList<Alarm>> FindActiveAlarmsAsync()
        {
            var activeAlarms = await _alarmsRepository.FindAsync(alarm => alarm.IsActive).ConfigureAwait(false);
            return activeAlarms.Where(IsActiveToday).ToList();
        }

        private IList<TriggeredAlarm> GetTriggeredAlarmsAsync(IList<GeofenceStateChangeReport> reports, IList<Alarm> alarms)
        {
            var activeGeofences = reports
                .Where(report => report.NewState == GeofenceState.Entered || report.NewState == GeofenceState.Exited)
                .Join(alarms, report => report.Geofence.Id,
                      alarm => alarm.Name.ToString(),
                     (report, alarm) => new TriggeredAlarm(report, alarm))
                .Distinct()
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

        private async Task ReregisterGeofences(IList<TriggeredAlarm> triggeredAlarms)
        {
            var geofences = triggeredAlarms
                .Where(alarm => !string.IsNullOrEmpty(alarm.Alarm.ActiveDays))
                .Select(alarm => alarm.Report.Geofence);

            foreach (var geofence in geofences)
                _geofenceService.ReplaceGeofence(geofence.Id, geofence);
        }

        private async Task UpdateAlarms() => await _alarmsRepository.UpdateAllAsync(_alarmsToUpdate.ToList()).ConfigureAwait(false);
    }
}