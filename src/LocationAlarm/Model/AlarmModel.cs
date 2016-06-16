using ArrivalAlarm.Model;
using System;
using System.Collections.Generic;
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
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

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
        [DataMember]
        public MonitoredArea MonitoredArea { get; set; }

        public AlarmModel()
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