using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Location;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.Utils;
using LocationAlarm.View;
using Microsoft.Practices.ServiceLocation;

namespace LocationAlarm.ViewModel
{
    public class ViewModelLocator
    {
        public AlarmSettingsViewModel AlarmSettings => ServiceLocator.Current.GetInstance<AlarmSettingsViewModel>();

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();

        public MapViewModel Map => ServiceLocator.Current.GetInstance<MapViewModel>();

        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            RegisterServices();

            SimpleIoc.Default.Register<LocationAlarmManager>();
            SimpleIoc.Default.Register<LocationAutoSuggestion>();
            SimpleIoc.Default.Register<GeolocationService>();
            SimpleIoc.Default.Register<MapViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AlarmSettingsViewModel>();
        }

        private void NavigationServiceConfiguration()
        {
            // Create navigation service object
            var navigationService = new NavigationService();

            // Add views to navigation service
            navigationService.Configure(nameof(MainPage), typeof(MainPage));
            navigationService.Configure(nameof(MapPage), typeof(MapPage));
            navigationService.Configure(nameof(AlarmSettingsPage), typeof(AlarmSettingsPage));

            var nav = new NavigationServiceWithToken(navigationService);

            SimpleIoc.Default.Register(() => nav);
        }

        private void RegisterServices()
        {
            var assetsNameReader = new AssetsNamesReader();
            SimpleIoc.Default.Register<IAssetsNamesReader>(() => assetsNameReader);

            NavigationServiceConfiguration();
        }
    }
}