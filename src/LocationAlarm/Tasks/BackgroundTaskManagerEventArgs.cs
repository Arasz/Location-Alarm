using System;
using System.Collections.Generic;

namespace LocationAlarm.Tasks
{
    public class BackgroundTaskManagerEventArgs : EventArgs
    {
        public IEnumerable<string> TasksNames { get; private set; }
    }
}