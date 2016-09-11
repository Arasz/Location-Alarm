using SQLite;
using System.Runtime.Serialization;

namespace CoreLibrary.Data.DataModel.PersistentModel
{
    /// <summary>
    /// Simple data model of alarm 
    /// </summary>
    [DataContract]
    public class Alarm
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
        public string AlarmSound { get; set; } = "default";

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
        /// Entity id 
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Alarm state 
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

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
        public string MapScreen { get; set; } = "";

        /// <summary>
        /// Alarm name 
        /// </summary>
        [DataMember]
        public string Name { get; set; } = "default alarm";

        /// <summary>
        /// Monitored area radius 
        /// </summary>
        [DataMember]
        public double Radius { get; set; } = 500;
    }
}