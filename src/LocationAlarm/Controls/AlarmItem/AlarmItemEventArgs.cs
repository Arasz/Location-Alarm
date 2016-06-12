using LocationAlarm.Model;
using System;

namespace LocationAlarm.Controls.AlarmItem
{
    public class AlarmItemEventArgs : EventArgs
    {
        public AlarmModel Source { get; }

        public AlarmItemEventArgs(AlarmModel source)
        {
            Source = source;
        }
    }
}