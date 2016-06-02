using ArrivalAlarm.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation.Metadata;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Location alarm data model 
    /// </summary>
    [DataContract]
    public class AlarmModel
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        [DataMember]
        public ISet<DayOfWeek> ActiveDays { get; set; } = new HashSet<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday, DayOfWeek.Saturday };

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Alarm cyclic 
        /// </summary>
        [DataMember]
        public bool IsCyclic { get; set; }

        /// <summary>
        /// Alarm label 
        /// </summary>
        [DataMember]
        public string Label { get; set; } = "DefaultLabel";

        /// <summary>
        /// Area on enter to which alarm will be activated 
        /// </summary>
        public MonitoredArea MonitoredArea { get; set; }

        /// <summary>
        /// User is informed only with notification 
        /// </summary>
        [DataMember]
        public bool OnlyNotifications { get; set; }

        /// <summary>
        /// Ringtone 
        /// </summary>
        [DataMember, Deprecated("Will change in the future", DeprecationType.Deprecate, 1)]
        public string Ringtone { get; set; } = "defaultRingtone";

        /// <summary>
        /// Snooze time after alarm sleep 
        /// </summary>
        [DataMember]
        public TimeSpan SnoozeTime { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="alarmLocation"> Location in which alarm will be triggered </param>
        public AlarmModel(MonitoredArea monitoredArea)
        {
            MonitoredArea = monitoredArea;
        }

        public AlarmModel()
        {
            Label = "Poznań";
            var builder = new GeofenceBuilder();
            builder.SetRequiredId(Label)
                .ThenSetGeocircle(new BasicGeoposition(), 4d)
                .ConfigureMonitoredStates(MonitoredGeofenceStates.Entered)
                .SetDwellTime(TimeSpan.FromMinutes(2));

            MonitoredArea = new MonitoredArea(Label, builder);
        }

        /// <summary>
        /// Activates alarm 
        /// </summary>
        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void Sleep()
        {
            throw new NotImplementedException();
        }
    }
}