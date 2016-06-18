using CoreLibrary.DataModel;
using System;

namespace LocationAlarm.Controls.AlarmItem
{
    public class AlarmItemEventArgs : EventArgs
    {
        public GeolocationAlarm Source { get; }

        public AlarmItemEventArgs(GeolocationAlarm source)
        {
            Source = source;
        }
    }
}