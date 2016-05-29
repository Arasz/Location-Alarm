using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Foundation.Metadata;

namespace LocationAlarm.Model
{
    /// <summary>
    /// Common interface for all alarms 
    /// </summary>
    public interface IAlarm
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        ISet<DayOfWeek> ActiveDays { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Alarm cyclic 
        /// </summary>
        bool IsCyclic { get; set; }

        /// <summary>
        /// Alarm label 
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// User is informed only with notification 
        /// </summary>
        bool OnlyNotifications { get; set; }

        /// <summary>
        /// Ringtone 
        /// </summary>
        string Ringtone { get; set; }

        /// <summary>
        /// Snooze time 
        /// </summary>
        DateTime SnoozeTime { get; set; }

        /// <summary>
        /// Activates alarm 
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates alarm 
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Turns on alarm snooze 
        /// </summary>
        void Sleep();
    }

    /// <summary>
    /// Common interface of objects which have alarm based on location functionality 
    /// </summary>
    public interface ILocationAlarm : IAlarm, ILocationMarker
    {
        /// <summary>
        /// Location where alarm will be triggered 
        /// </summary>
        AlarmLocation AlarmLocation { get; set; }
    }

    /// <summary>
    /// Common interface for objects which have location marker for monitoring purpose 
    /// </summary>
    public interface ILocationMarker
    {
        /// <summary>
        /// Monitored location 
        /// </summary>
        Geofence LocationMarker { get; set; }
    }

    /// <summary>
    /// Location alarm data model 
    /// </summary>
    [DataContract]
    public class AlarmModel : ILocationAlarm
    {
        /// <summary>
        /// Days in which alarm is active 
        /// </summary>
        [DataMember]
        public ISet<DayOfWeek> ActiveDays { get; set; } = new HashSet<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Friday, DayOfWeek.Saturday };

        /// <summary>
        /// Location where alarm will be triggered 
        /// </summary>
        [DataMember]
        public AlarmLocation AlarmLocation { get; set; } = new AlarmLocation("DefaultLocation", new BasicGeoposition() { Latitude = 52.403343, Longitude = 16.950777 });

        /// <summary>
        /// Alarm id 
        /// </summary>
        [DataMember]
        public long Id { get; set; }

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
        public Geofence LocationMarker { get; set; }

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
        public DateTime SnoozeTime { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="alarmLocation"> Location in which alarm will be triggered </param>
        public AlarmModel(AlarmLocation alarmLocation)
        {
            AlarmLocation = alarmLocation;

            LocationMarker = new Geofence(AlarmLocation.GetHashCode().ToString(), new Geocircle(AlarmLocation.Geoposition, alarmLocation.Radius), MonitoredGeofenceStates.Entered, IsCyclic, TimeSpan.FromSeconds(30));

            Id = UniqueIdProvider.Id;
        }

        public AlarmModel()
        {
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

        public override string ToString()
        {
            StringBuilder textRepresentation = new StringBuilder();

            textRepresentation.AppendLine($"Label: {Label}");
            textRepresentation.AppendLine($"IsActive: {IsActive}");
            textRepresentation.AppendLine($"Location:\n{AlarmLocation}");
            textRepresentation.Append($"Active days: ");
            foreach (var activeDay in ActiveDays)
                textRepresentation.Append($"{activeDay} ");
            textRepresentation.AppendLine();
            return textRepresentation.ToString();
        }
    }

    public class UniqueIdProvider
    {
        public static long Id { get; set; }
    }
}