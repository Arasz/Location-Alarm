using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LocationAlarm.Tasks
{
    public class BackgroundTaskManager
    {
        public event EventHandler TaskCompleted;

        public event EventHandler TaskProgress;

        public BackgroundAccessStatus BackgroundAccessStatus { get; private set; }

        public IBackgroundTaskRegistration FetchBackgroundTaskRegistration(string taskName)
            => BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(registration => registration.Name == taskName);

        public async Task RegisterBackgroundTaskAsync(Type taskType)
        {
            await RegisterBackgroundTaskAsync(taskType.Name, taskType.FullName).ConfigureAwait(false);
        }

        public async Task RegisterBackgroundTaskAsync(string taskName, string taskEntryPoint)
        {
            BackgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (IsTaskRegistered(taskName)) return;

            var backgroundTaskBuilder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint,
            };

            var locationTrigger = new LocationTrigger(LocationTriggerType.Geofence);

            backgroundTaskBuilder.SetTrigger(locationTrigger);

            var registration = backgroundTaskBuilder.Register();

            SubscribeBackgroundTaskEvents(registration);
        }

        public void UnregisterTask(string taskName)
        {
            if (!IsTaskRegistered(taskName)) return;

            var taskToUnregister = FetchBackgroundTaskRegistration(taskName);

            taskToUnregister?.Unregister(true);
        }

        protected virtual void OnTaskCompleted()
        {
            TaskCompleted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTaskProgress()
        {
            TaskProgress?.Invoke(this, EventArgs.Empty);
        }

        private bool IsTaskRegistered(string taskName)
        {
            var registeredTask = FetchBackgroundTaskRegistration(taskName);

            return registeredTask != null;
        }

        private void OnRegisteredTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            OnTaskCompleted();
        }

        private void OnRegisteredTaskProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            OnTaskProgress();
        }

        private void SubscribeBackgroundTaskEvents(IBackgroundTaskRegistration registration)
        {
            registration.Completed += OnRegisteredTaskCompleted;
            registration.Progress += OnRegisteredTaskProgress;
        }
    }
}