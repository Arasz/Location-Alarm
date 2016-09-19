using CoreLibrary.Data.DataModel.Base;
using System.Runtime.Serialization;

namespace CoreLibrary.Logger
{
    [DataContract]
    public class Log : Entity
    {
        [DataMember]
        public string ExceptionMessage { get; set; }

        [DataMember]
        public string Exceptions { get; set; }

        [DataMember]
        public string Level { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Type { get; set; }
    }
}