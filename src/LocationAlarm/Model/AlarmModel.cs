using ArrivalAlarm.Model;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Foundation.Metadata;
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
        public ISet<DayOfWeek> ActiveDays { get; set; } = new HashSet<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday, DayOfWeek.Saturday };

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary> Alarm cyclic </summary.
        [DataMember]
        public bool IsCyclic { get; set; }

        /// <summary>
        /// Alarm label 
        /// </summary>
        [DataMember, Deprecated("Label is deprecated", DeprecationType.Deprecate, 0)]
        public string Label { get; set; } = "DefaultLabel";

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
        /// Ringtone 
        /// </summary>
        [DataMember, Deprecated("Will change in the future", DeprecationType.Deprecate, 1)]
        public string Ringtone { get; set; } = "defaultRingtone";

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
}