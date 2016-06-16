using LocationAlarm.Model;
using System;

namespace LocationAlarm.Controls.AlarmItem
{
    public class AlarmItemEventArgs : EventArgs
    {
        public Model.LocationAlarm Source { get; }

        public AlarmItemEventArgs(Model.LocationAlarm source)
        {
            Source = source;
        }
    }
}