namespace BackgroundTask.Toast
{
    /// <summary>
    /// Can mange notifications 
    /// </summary>
    internal interface IAlarmNotificationService
    {
        /// <summary>
        /// Sends notification to user 
        /// </summary>
        void Notify();
    }
}