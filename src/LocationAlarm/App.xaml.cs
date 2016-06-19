using Autofac;
using GalaSoft.MvvmLight.Threading;
using LocationAlarm.Utils;
using System;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
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
        private IContainer _container;
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
            //ConfigureContainer();
            DispatcherHelper.Initialize();
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

        private void ConfigureContainer()
        {
            var containerBuilder = new ContainerBuilder();
            var assembly = typeof(App).GetTypeInfo().Assembly;
            containerBuilder.RegisterAssemblyTypes(assembly)
                .AsSelf()
                .AsImplementedInterfaces();

            containerBuilder.RegisterType<AssetsNamesReader>()
                .SingleInstance()
                .AsImplementedInterfaces();

            _container = containerBuilder.Build();
        }
    }
}