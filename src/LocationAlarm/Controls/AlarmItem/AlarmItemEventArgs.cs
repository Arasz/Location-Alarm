using CoreLibrary.Data.DataModel.PersistentModel;
using System;

namespace LocationAlarm.Controls.AlarmItem
{
    public class AlarmItemEventArgs : EventArgs
    {
        public Alarm Source { get; }

        public AlarmItemEventArgs(Alarm source)
        {
            Source = source;
        }
    }
}