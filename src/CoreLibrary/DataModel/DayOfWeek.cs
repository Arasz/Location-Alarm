using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace CoreLibrary.DataModel
{
    [DataContract, Equals]
    public class WeekDay : Entity
    {
        [DataMember]
        public DayOfWeek Day { get; set; }

        public static string[] DayNames => DateTimeFormatInfo.CurrentInfo.DayNames;

        [DataMember]
        public string Name { get; }

        [DataMember]
        public string ShortName { get; }

        public WeekDay()
        {
        }

        public WeekDay(string name)
        {
            if (!DayNames.Contains(name)) throw new ArgumentException($"Wrong day of week name: {name}. {nameof(WeekDay)} object construction failure");

            Day = (DayOfWeek)DayNames.ToList().IndexOf(name);
            Name = DayNames[(int)Day];
            ShortName = Name.Substring(0, 3);
        }

        public WeekDay(DayOfWeek day)
        {
            Day = day;
            Name = DayNames[(int)Day];
            ShortName = Name.Substring(0, 3);
        }

        public override string ToString() => Name;
    }
}