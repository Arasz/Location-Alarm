using System.Collections.Generic;

namespace BackgroundTask
{
    internal class TriggeredAlarmEqualityComparer : IEqualityComparer<TriggeredAlarm>
    {
        public bool Equals(TriggeredAlarm x, TriggeredAlarm y) => x.Alarm.Id == y.Alarm.Id;

        public int GetHashCode(TriggeredAlarm obj) => obj.Alarm.Id;
    }
}