using CoreLibrary.Data;
using CoreLibrary.Data.DataModel;
using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.StateManagement;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace CoreLibrary.DataModel
{
    /// <summary>
    /// Location alarm data model 
    /// </summary>
    [DataContract, Equals]
    public class GeolocationAlarm : IEntity, IRestorable<GeolocationAlarm>
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        [DataMember]
        public List<WeekDay> ActiveDays { get; set; }

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

        public Geofence Geofence => new GeofenceBuilder()
                                        .WithDefaultParameters()
                                        .SetRequiredId(Name)
                                        .ThenSetGeocircle(Geoposition, Radius)
                                        .IsUsedOnce(!ActiveDays.Any())
                                        .Build();

        /// <summary>
        /// Geographic position 
        /// </summary>
        [DataMember]
        public BasicGeoposition Geoposition { get; set; }

        public int Id { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Path to map screen 
        /// </summary>
        public string MapScreenPath { get; set; }

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
            ActiveDays = new List<WeekDay>(7);
            AlarmSound = "default";
            AlarmType = AlarmType.Notification;
            Radius = 500;
        }

        public void Restore(GeolocationAlarm savedState)
        {
            ActiveDays = savedState.ActiveDays.ToList();
            AlarmSound = savedState.AlarmSound;
            AlarmType = savedState.AlarmType;
            Geoposition = savedState.Geoposition;
            IsActive = savedState.IsActive;
            MapScreenPath = savedState.MapScreenPath;
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
                MapScreenPath = MapScreenPath,
                Name = Name,
                Radius = Radius,
            };
        }
    }
}