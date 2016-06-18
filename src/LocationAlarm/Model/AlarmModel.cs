using CoreLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Location alarm data model 
    /// </summary>
    [DataContract, Equals]
    public class AlarmModel : Entity
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        [DataMember]
        public List<DayOfWeek> ActiveDays { get; set; }

        /// <summary>
        /// Ringtone 
        /// </summary>
        [DataMember]
        public string AlarmSound { get; set; }

        /// <summary>
        /// Type of alarm 
        /// </summary>
        [DataMember]
        public AlarmType AlarmType { get; set; }

        /// <summary>
        /// Geographic position 
        /// </summary>
        [DataMember]
        public BasicGeoposition Geoposition { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public BitmapImage MapScreen { get; set; }

        /// <summary>
        /// States in which alarm will activate 
        /// </summary>
        [DataMember]
        public MonitoredGeofenceStates MonitoredStates { get; set; }

        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        [DataMember]
        public double Radius { get; set; }

        public AlarmModel()
        {
            ActiveDays = new List<DayOfWeek>(7);
            AlarmSound = "default";
            AlarmType = AlarmType.Notification;
            Radius = 500;
            MonitoredStates = MonitoredGeofenceStates.Entered;
        }
    }

    public enum AlarmType
    {
        Notification,
        Sound,
    }
}