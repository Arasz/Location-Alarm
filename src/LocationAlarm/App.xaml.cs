using Autofac;
using Autofac.Core.Activators.Reflection;
using BackgroundTask;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Geofencing;
using CoreLibrary.Data.Persistence.Repository;
using CoreLibrary.Service;
using CoreLibrary.Service.Geofencing;
using CoreLibrary.Service.Geolocation;
using CoreLibrary.Utils.AssetsReader;
using CoreLibrary.Utils.ScreenshotManager;
using GalaSoft.MvvmLight.Threading;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.BackgroundTask;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using LocationAlarm.ViewModel;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using MainPage = LocationAlarm.View.MainPage;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace ArrivalAlarm
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class. 
    /// </summary>
    public sealed partial class App : Application
    {
        public static IContainer Container { get; set; }

        private BackgroundTaskManager<GeofenceTask> _backgroundTaskManager;

        private TransitionCollection transitions;

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            InitializeIocContainer();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Debug.WriteLine($"Message: {unhandledExceptionEventArgs.Message}");
            Debug.WriteLine($"Stack trace: {unhandledExceptionEventArgs.Exception.StackTrace}");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user. Other entry points
        /// will be used when the application is launched to open a specific file, to display search
        /// results, and so forth.
        /// </summary>
        /// <param name="e"> Details about the launch request and process. </param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Resources["Locator"] = Container.Resolve<ViewModelLocator>();
            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content, just ensure that
            // the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 1;

                // Set the default language
                rootFrame.Language = ApplicationLanguages.Languages[0];

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page, configuring
                // the new page by passing required information as a navigation parameter
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Ensure the current window is active
            Window.Current.Activate();

            //Configure dispatcher
            DispatcherHelper.Initialize();

            RegisterBackgroundTaskAsync();
        }

        private async Task RegisterBackgroundTaskAsync()
        {
            if (_backgroundTaskManager == null)
                _backgroundTaskManager = Container.Resolve<BackgroundTaskManager<GeofenceTask>>();

            await _backgroundTaskManager.RegisterBackgroundTaskAsync().ConfigureAwait(false);
        }

        private static void InitializeIocContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<GeofenceService>()
                .As<IGeofenceService>();

            builder.RegisterType<GeofenceBuilder>()
                .AsSelf();

            builder.RegisterInstance(ApplicationData.Current.LocalFolder)
                .As<IStorageFolder>()
                .Named<IStorageFolder>("defaultFolder");

            builder.RegisterType<GeolocationService>()
                .As<IGeolocationService>();

            builder.RegisterType<GenericRepository<Alarm>>()
                .As<IRepository<Alarm>>()
                .SingleInstance();

            builder.RegisterType<ScreenshotManager>()
                .SingleInstance()
                .AsImplementedInterfaces()
                .WithParameter(new AutowiringParameter());

            builder.RegisterType<BackgroundTaskManager<GeofenceTask>>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<AssetsNamesReader>()
                .As<IAssetsNamesReader>()
                .SingleInstance();

            builder.RegisterType<LocationAlarmModel>()
                .AsSelf();

            builder.RegisterType<LocationAutoSuggestion>()
                .AsSelf();

            builder.RegisterType<MapViewModel>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MainViewModel>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<AlarmSettingsViewModel>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<NavigationService>()
                .As<INavigationService>()
                .AsSelf()
                .SingleInstance()
                .OnActivated(activatedEventArgs =>
                {
                    var navigationService = activatedEventArgs.Instance;
                    navigationService.Configure(nameof(MainPage), typeof(MainPage));
                    navigationService.Configure(nameof(MapPage), typeof(MapPage));
                    navigationService.Configure(nameof(AlarmSettingsPage), typeof(AlarmSettingsPage));
                });

            builder.RegisterType<NavigationServiceWithToken>()
                .AsSelf().SingleInstance();

            builder.RegisterType<ViewModelLocator>()
                .AsSelf()
                .SingleInstance();

            Container = builder.Build();
        }

        /// <summary>
        /// Restores the content <see cref="transitions"/> after the app has launched. 
        /// </summary>
        /// <param name="sender"> The object where the handler is attached. </param>
        /// <param name="e"> Details about the navigation event. </param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            rootFrame.ContentTransitions = transitions ?? new TransitionCollection { new NavigationThemeTransition() };
            rootFrame.Navigated -= RootFrame_FirstNavigated;
        }

        /// <summary>
        /// Invoked when application execution is being suspended. <see cref="Application"/> state is
        /// saved without knowing whether the application will be terminated or resumed with the
        /// contents of memory still intact.
        /// </summary>
        /// <param name="sender"> The source of the suspend request. </param>
        /// <param name="e"> Details about the suspend request. </param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}