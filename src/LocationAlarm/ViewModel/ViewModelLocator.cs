using CoreLibrary.DataModel;
using CoreLibrary.Service;
using CoreLibrary.Service.Geofencing;
using CoreLibrary.Service.Geolocation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.Utils;
using LocationAlarm.View;

namespace LocationAlarm.ViewModel
{
    public class ViewModelLocator
    {
        public AlarmSettingsViewModel AlarmSettings => SimpleIoc.Default.GetInstance<AlarmSettingsViewModel>();

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        public MapViewModel Map => SimpleIoc.Default.GetInstance<MapViewModel>();

        public ViewModelLocator()
        {
            RegisterServices();

            SimpleIoc.Default.Register<IGeofenceService, GeofenceService>();
            SimpleIoc.Default.Register<GelocationAlarmRepository>();
            SimpleIoc.Default.Register<LocationAlarmModel>();
            SimpleIoc.Default.Register<LocationAutoSuggestion>();
            SimpleIoc.Default.Register<IGeolocationService, GeolocationService>();
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