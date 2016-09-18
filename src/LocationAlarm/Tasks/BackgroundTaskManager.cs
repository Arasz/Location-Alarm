using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LocationAlarm.BackgroundTask
{
    public class BackgroundTaskManager<TTask>
        where TTask : IBackgroundTask
    {
        private IBackgroundTaskRegistration _registeredTask;

        public event EventHandler TaskCompleted;

        public event EventHandler<int> TaskProgress;

        public BackgroundAccessStatus BackgroundAccessStatus { get; private set; }

        public bool IsTaskRegistered => BackgroundTaskRegistration.AllTasks.Values
            .Any(taskRegistration => taskRegistration.Name == typeof(TTask).Name);

        public IBackgroundTaskRegistration RegisteredTask => _registeredTask ?? (_registeredTask = BackgroundTaskRegistration.AllTasks.Values
                    .FirstOrDefault(registration => registration.Name == typeof(TTask).Name));

        public async Task RegisterBackgroundTaskAsync() => await RegisterBackgroundTaskAsync(typeof(TTask)).ConfigureAwait(false);

        public void UnregisterTask()
        {
            if (!IsTaskRegistered)
                return;
            var taskToUnregister = RegisteredTask;
            taskToUnregister?.Unregister(true);
            _registeredTask = null;
        }

        private void OnRegisteredTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args) => OnTaskCompleted();

        private void OnRegisteredTaskProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args) => OnTaskProgress((int)args.Progress);

        private void OnTaskCompleted() => TaskCompleted?.Invoke(this, EventArgs.Empty);

        private void OnTaskProgress(int progress) => TaskProgress?.Invoke(this, progress);

        private async Task RegisterBackgroundTaskAsync(Type taskType) => await RegisterBackgroundTaskAsync(taskType.Name, taskType.FullName).ConfigureAwait(false);

        private async Task RegisterBackgroundTaskAsync(string taskName, string taskEntryPoint)
        {
            BackgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (IsTaskRegistered)
            {
                SubscribeBackgroundTaskEvents(RegisteredTask);
                return;
            }

            var backgroundTaskBuilder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint,
            };

            var locationTrigger = new LocationTrigger(LocationTriggerType.Geofence);

            backgroundTaskBuilder.SetTrigger(locationTrigger);

            backgroundTaskBuilder.Register();

            SubscribeBackgroundTaskEvents(RegisteredTask);
        }

        private void SubscribeBackgroundTaskEvents(IBackgroundTaskRegistration registration)
        {
            registration.Completed += OnRegisteredTaskCompleted;
            registration.Progress += OnRegisteredTaskProgress;
        }
    }
}