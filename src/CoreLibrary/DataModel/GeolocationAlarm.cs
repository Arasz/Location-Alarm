﻿using CoreLibrary.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Xaml.Media.Imaging;

namespace CoreLibrary.DataModel
{
    /// <summary>
    /// Location alarm data model 
    /// </summary>
    [DataContract, Equals]
    public class GeolocationAlarm : Entity, IRestorable<GeolocationAlarm>
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

        public GeolocationAlarm()
        {
            Name = "";
            ActiveDays = new List<DayOfWeek>(7);
            AlarmSound = "default";
            AlarmType = AlarmType.Notification;
            Radius = 500;
            MonitoredStates = MonitoredGeofenceStates.Entered;
        }

        public void Restore(GeolocationAlarm savedState)
        {
            ActiveDays = savedState.ActiveDays.ToList();
            AlarmSound = savedState.AlarmSound;
            AlarmType = savedState.AlarmType;
            Geoposition = savedState.Geoposition;
            IsActive = savedState.IsActive;
            MapScreen = savedState.MapScreen;
            MonitoredStates = savedState.MonitoredStates;
            Name = savedState.Name;
            Radius = savedState.Radius;
        }

        public GeolocationAlarm Save()
        {
            return new GeolocationAlarm()
            {
                ActiveDays = ActiveDays.ToList(),
                AlarmSound = AlarmSound,
                AlarmType = AlarmType,
                Geoposition = Geoposition,
                IsActive = IsActive,
                MapScreen = MapScreen,
                MonitoredStates = MonitoredStates,
                Name = Name,
                Radius = Radius,
            };
        }
    }

    public enum AlarmType
    {
        Notification,
        Sound,
    }
}