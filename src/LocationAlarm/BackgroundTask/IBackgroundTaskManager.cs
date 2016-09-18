using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LocationAlarm.BackgroundTask
{
    public interface IBackgroundTaskManager
    {
        /// <summary>
        /// Fired when background task ends its execution 
        /// </summary>
        event EventHandler TaskCompleted;

        /// <summary>
        /// Fired when task execution progress changes 
        /// </summary>
        event EventHandler<int> TaskProgress;

        /// <summary>
        /// Application ability to perform background activity 
        /// </summary>
        BackgroundAccessStatus BackgroundAccessStatus { get; }

        /// <summary>
        /// Registered task. If task isn't registered returns <see langword="null"/> 
        /// </summary>
        IBackgroundTaskRegistration RegisteredTask { get; }

        /// <summary>
        /// Registers task 
        /// </summary>
        /// <returns></returns>
        Task RegisterBackgroundTaskAsync();

        /// <summary>
        /// Unregister task 
        /// </summary>
        void UnregisterTask();
    }
}