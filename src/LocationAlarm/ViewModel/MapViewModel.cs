using ArrivalAlarm.Messages;
using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Location;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using Microsoft.Practices.ServiceLocation;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MapViewModel : ViewModelBase, INavigable
    {
        private readonly MapModel _mapModel;

        /// <summary>
        /// Navigation service used for navigation between pages 
        /// </summary>
        private readonly INavigationService _navigationService;

        private LocationAutoSuggestion _autoSuggestion;

        public Geopoint ActualLocation { get; private set; }

        /// <summary>
        /// State of map loading 
        /// </summary>
        public bool IsMapLoaded { get; private set; }

        /// <summary>
        /// Text from auto suggest box 
        /// </summary>
        public string LocationQuery
        {
            get { return _autoSuggestion.ProvidedLocationQuery; }
            set { _autoSuggestion.ProvidedLocationQuery = value; }
        }

        /// <summary>
        /// Map control screen shoot 
        /// </summary>
        public BitmapImage MapScreenshot { get; set; }

        public bool PushpinVisible { get; private set; }

        public ObservableCollection<ReadableLocationName> SuggestedLocations => _autoSuggestion.SuggestedLocations;

        /// <summary>
        /// </summary>
        public ICommand SuggestionChosenCommand { get; private set; }

        /// <summary>
        /// Command launched when user clicked combo box 
        /// </summary>
        public ICommand TextChangeCommand { get; private set; }

        /// <summary>
        /// Map zoom level (from 1 to 20 ) 
        /// </summary>
        public double ZoomLevel
        {
            get; private set;
        }

        public MapViewModel(MapModel mapModel)
        {
            _autoSuggestion = new LocationAutoSuggestion(mapModel);
            _mapModel = mapModel;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            _autoSuggestion.SuggestionSelected += AutoSuggestionOnSuggestionSelected;

            TextChangeCommand = _autoSuggestion.TextChangedCommand;
            SuggestionChosenCommand = _autoSuggestion.SuggestionChosenCommand;

            Messenger.Default.Register<bool>(this, Tokens.MapLoaded, (isMapLoaded) => IsMapLoaded = isMapLoaded);
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(object parameter)
        {
            IsMapLoaded = false;
            MapScreenshot = null;
        }

        public void OnNavigatedTo(object parameter)
        {
            IsMapLoaded = false;
            MapScreenshot = null;
            UpdateUserLocation();
        }

        private void AutoSuggestionOnSuggestionSelected(object sender, MapLocation selectedLocation)
        {
            Messenger.Default.Send(new MapMessage(), Tokens.FocusOnMap);
            SetProvidedLocation(selectedLocation);
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            await TakeMapScreenshotAsync().ConfigureAwait(true);
            object locationData = MapScreenshot;

            _navigationService.NavigateTo(nameof(View.AlarmSettingsPage), locationData);
        }

        private void SetProvidedLocation(MapLocation location)
        {
            if (location == null)
                return;

            ActualLocation = location.Point;
            ZoomLevel = 12;
            Messenger.Default.Send(ActualLocation, Tokens.SetMapView);
            PushpinVisible = true;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private async Task TakeMapScreenshotAsync()
        {
            Messenger.Default.Send(new MapMessage(), Tokens.TakeScreenshot);

            await Task.Factory.StartNew(() =>
            {
                while (MapScreenshot == null)
                {
                }
            }).ConfigureAwait(true);
        }

        /// <summary>
        /// Updates actual user location 
        /// </summary>
        [OnCommand("FindMeCommand")]
        private async void UpdateUserLocation()
        {
            var actualLocation = await _mapModel.GetActualLocationAsync().ConfigureAwait(true);
            ActualLocation = actualLocation.Coordinate.Point;
            ZoomLevel = 12;
            Messenger.Default.Send(ActualLocation, Tokens.SetMapView);
            PushpinVisible = true;
        }
    }
}