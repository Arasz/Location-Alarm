using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LocationAlarm.Tasks
{
    public class BackgroundTaskManager
    {
        private IBackgroundTaskRegistration _registeredTask;

        public event EventHandler TaskCompleted;

        public event EventHandler<int> TaskProgress;

        public BackgroundAccessStatus BackgroundAccessStatus { get; private set; }

        private IBackgroundTaskRegistration RegisteredTask => _registeredTask;

        public BackgroundTaskManager()
        {
        }

        public IBackgroundTaskRegistration FetchBackgroundTaskRegistration(string taskName)
                    => BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(registration => registration.Name == taskName);

        public async Task RegisterBackgroundTaskAsync(Type taskType) => await RegisterBackgroundTaskAsync(taskType.Name, taskType.FullName).ConfigureAwait(false);

        public async Task RegisterBackgroundTaskAsync(string taskName, string taskEntryPoint)
        {
            BackgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            if (IsTaskRegistered(taskName, out _registeredTask))
            {
                SubscribeBackgroundTaskEvents(_registeredTask);
                return;
            }

            var allTasks = BackgroundTaskRegistration.AllTasks.ToDictionary(pair => pair.Key, pair => pair.Value);

            var backgroundTaskBuilder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint,
            };

            var locationTrigger = new LocationTrigger(LocationTriggerType.Geofence);

            backgroundTaskBuilder.SetTrigger(locationTrigger);

            _registeredTask = backgroundTaskBuilder.Register();

            SubscribeBackgroundTaskEvents(_registeredTask);
        }

        public void UnregisterTask(string taskName)
        {
            var taskToUnregister = default(IBackgroundTaskRegistration);
            if (!IsTaskRegistered(taskName, out taskToUnregister)) return;

            taskToUnregister?.Unregister(true);
        }

        private bool IsTaskRegistered(string taskName, out IBackgroundTaskRegistration registeredTask)
        {
            registeredTask = FetchBackgroundTaskRegistration(taskName);

            return registeredTask != null;
        }

        private void OnRegisteredTaskCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args) => OnTaskCompleted();

        private void OnRegisteredTaskProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args) => OnTaskProgress((int)args.Progress);

        private void OnTaskCompleted() => TaskCompleted?.Invoke(this, EventArgs.Empty);

        private void OnTaskProgress(int progress) => TaskProgress?.Invoke(this, progress);

        private void SubscribeBackgroundTaskEvents(IBackgroundTaskRegistration registration)
        {
            registration.Completed += OnRegisteredTaskCompleted;
            registration.Progress += OnRegisteredTaskProgress;
        }
    }
}