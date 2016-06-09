﻿using System;
using System.Runtime.Serialization;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;

namespace ArrivalAlarm.Model
{
    /// <summary>
    /// Geofence class decorator 
    /// </summary>
    [DataContract]
    public class MonitoredArea
    {
        /// <summary>
        /// Decorated geofence object 
        /// </summary>
        [DataMember]
        private Geofence _geofence;

        /// <summary>
        /// Gets the time window, beginning after the StartTime, during which the Geofence is monitored 
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// The minimum time that a position has to be inside or outside of the Geofence in order for
        /// the notification to be triggered.
        /// </summary>
        public TimeSpan DwellTime { get; set; }

        /// <summary>
        /// Alarm position on map 
        /// </summary>
        [DataMember]
        public Geopoint Geopoint { get; set; }

        /// <summary>
        /// Alarm position on map 
        /// </summary>
        [DataMember]
        public BasicGeoposition Geoposition { get; set; }

        /// <summary>
        /// The shape of the geofence region 
        /// </summary>
        public IGeoshape Geoshape => _geofence.Geoshape;

        /// <summary>
        /// The id of the Geofence 
        /// </summary>
        public string Id => Name;

        /// <summary>
        /// Indicates the states that the geofence is being monitored for 
        /// </summary>
        public MonitoredGeofenceStates MonitoredStates => _geofence.MonitoredStates;

        /// <summary>
        /// Monitored area name 
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        [DataMember]
        public double Radius { get; set; } = 500;

        /// <summary>
        /// Indicates whether the Geofence should be triggered once or multiple times. 
        /// </summary>
        public bool SingleUse { get; set; }

        /// <summary>
        /// The time to start monitoring the Geofence. 
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="name"> Location <c> name </c> </param>
        /// <param name="geoposition"> Position on map in geographic coordinates </param>
        /// <param name="geocircleRadius"> Geocircle (geofence) radius in meters </param>
        public MonitoredArea()
        {
        }

        public static explicit operator Geofence(MonitoredArea monitoredArea)
        {
            return monitoredArea._geofence;
        }

        public static implicit operator MonitoredArea(Geofence geofence)
        {
            //return new MonitoredArea("", new GeofenceBuilder(geofence));
            return new MonitoredArea();
        }
    }
}