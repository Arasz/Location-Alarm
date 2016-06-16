﻿using Windows.ApplicationModel.Background;

namespace BackgroundTask
{
    /// <summary>
    /// Interface of task which can manage geofences and geolocation notifications 
    /// </summary>
    public interface IGeofenceTask
    {
        void Run(IBackgroundTaskInstance instance);
    }
}