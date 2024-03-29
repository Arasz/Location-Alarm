﻿using CoreLibrary.Data.DataModel.Base;
using SQLite;
using System.Runtime.Serialization;

namespace CoreLibrary.Data.DataModel.PersistentModel
{
    /// <summary>
    /// Simple data model of alarm 
    /// </summary>
    [DataContract, ToString]
    public class Alarm : Entity
    {
        /// <summary>
        /// Days in which alarm is active. Each day is separated by , 
        /// </summary>
        [DataMember]
        public string ActiveDays { get; set; } = "";

        /// <summary>
        /// Alarm sound name 
        /// </summary>
        [DataMember]
        public string AlarmSound { get; set; } = "ms-winsoundevent:Notification.Default";

        /// <summary>
        /// Type of alarm 
        /// </summary>
        [DataMember]
        public AlarmType AlarmType { get; set; } = AlarmType.Notification;

        /// <summary>
        /// The altitude of the geographic position 
        /// </summary>
        [DataMember]
        public double Altitude { get; set; }

        /// <summary>
        /// Was alarm fired 
        /// </summary>
        [DataMember]
        public bool Fired { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// The latitude of the geographic position. 
        /// </summary>
        [DataMember]
        public double Latitude { get; set; }

        /// <summary>
        /// The latitude of the geographic position. 
        /// </summary>
        [DataMember]
        public double Longitude { get; set; }

        /// <summary>
        /// Path to map screen 
        /// </summary>
        [DataMember]
        public string MapScreenPath { get; set; }

        /// <summary>
        /// Alarm name 
        /// </summary>
        [DataMember, Unique]
        public string Name { get; set; } = "";

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        [DataMember]
        public double Radius { get; set; } = 500;

        public Alarm Clone() => MemberwiseClone() as Alarm;
    }
}