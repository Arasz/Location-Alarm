using SQLite;
using SQLiteNetExtensions.Attributes;
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
    public class LocationAlarm
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        [DataMember, OneToMany]
        public ISet<DayOfWeek> ActiveDays { get; set; } = new SortedSet<DayOfWeek>();

        /// <summary>
        /// Ringtone 
        /// </summary>
        [DataMember]
        public string AlarmSound { get; set; } = "default";

        /// <summary>
        /// Type of alarm 
        /// </summary>
        [DataMember]
        public AlarmType AlarmType { get; set; } = AlarmType.Notification;

        /// <summary>
        /// Alarm id 
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary> Alarm cyclic </summary.
        [DataMember]
        public bool IsCyclic => !ActiveDays.Any();

        [DataMember]
        public string Label => MonitoredArea.Name;

        /// <summary>
        /// Selected location screen shot 
        /// </summary>
        [DataMember]
        public BitmapImage MapScreen { get; set; }

        /// <summary>
        /// Area on enter to which alarm will be activated 
        /// </summary>
        [DataMember, OneToOne]
        public MonitoredArea MonitoredArea { get; set; }

        /// <summary>
        /// Snooze time after alarm sleep 
        /// </summary>
        [DataMember]
        public TimeSpan SnoozeTime { get; set; }

        public LocationAlarm()
        {
            MonitoredArea = new MonitoredArea();
        }
    }

    public enum AlarmType
    {
        Notification,
        Sound,
    }
}