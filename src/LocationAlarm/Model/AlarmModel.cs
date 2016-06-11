using ArrivalAlarm.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media.Imaging;

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
        public ISet<DayOfWeek> ActiveDays { get; set; } = new HashSet<DayOfWeek>()
        {
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        };

        /// <summary>
        /// Ringtone 
        /// </summary>
        public string AlarmSound { get; set; } = "default";

        /// <summary>
        /// Type of alarm 
        /// </summary>
        public AlarmType AlarmType { get; set; } = AlarmType.Sound;

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary> Alarm cyclic </summary.
        [DataMember]
        public bool IsCyclic => !ActiveDays.Any();

        public string Label => MonitoredArea.Name;

        /// <summary>
        /// Selected location screen shot 
        /// </summary>
        public BitmapImage MapScreen { get; set; }

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
        /// Snooze time after alarm sleep 
        /// </summary>
        [DataMember]
        public TimeSpan SnoozeTime { get; set; }

        public AlarmModel()
        {
            MonitoredArea = new MonitoredArea();
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

    public enum AlarmType
    {
        Notification,
        Sound,
    }
}