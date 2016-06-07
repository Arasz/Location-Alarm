/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ArrivalAlarm"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Location;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Model;
using LocationAlarm.Utils;
using LocationAlarm.View;
using Microsoft.Practices.ServiceLocation;

namespace LocationAlarm.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the application and provides
    /// an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        public AlarmSettingsViewModel AlarmSettings => ServiceLocator.Current.GetInstance<AlarmSettingsViewModel>();

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public MapViewModel Map => ServiceLocator.Current.GetInstance<MapViewModel>();

        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class. 
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();

            SimpleIoc.Default.Register<ILocationNameExtractor, LocationNameExtractor>();

            SimpleIoc.Default.Register<LocationAutoSuggestion>();
            SimpleIoc.Default.Register<MapModel>();
            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AlarmSettingsViewModel>();
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

        /// <summary>
        /// Creates and configures instance of navigation service 
        /// </summary>
        private void NavigationServiceConfiguration()
        {
            // Create navigation service object
            NavigationService navigationService = new NavigationService();

            // Add views to navigation service
            navigationService.Configure(nameof(MainPage), typeof(MainPage));
            navigationService.Configure(nameof(MapPage), typeof(MapPage));
            navigationService.Configure(nameof(AlarmSettingsPage), typeof(AlarmSettingsPage));

            // Register navigation service (object)
            SimpleIoc.Default.Register<INavigationService>(() => navigationService);
        }

        /// <summary>
        /// Registers all services 
        /// </summary>
        private void RegisterServices()
        {
            var assetsNameReader = new AssetsNamesReader();
            SimpleIoc.Default.Register<IAssetsNamesReader>(() => assetsNameReader);

            NavigationServiceConfiguration();
        }
    }
}