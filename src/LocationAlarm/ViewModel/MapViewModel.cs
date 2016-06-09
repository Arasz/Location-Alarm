using ArrivalAlarm.Model;
using Commander;
using GalaSoft.MvvmLight.Messaging;
using LocationAlarm.Common;
using LocationAlarm.Location;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MapViewModel : ViewModelBaseEx
    {
        private readonly LocationAutoSuggestion _autoSuggestion;
        private readonly GeolocationModel _geolocationModel;
        private MonitoredArea _monitoredArea;
        private MonitoredArea _monitoredAreaCopy;

        public Geopoint ActualLocation
        {
            get { return _monitoredArea.Geopoint; }
            private set { _monitoredArea.Geopoint = value; }
        }

        public string AutoSuggestionLocationQuery
        {
            get
            {
                return _autoSuggestion.ProvidedLocationQuery;
            }
            set
            {
                _monitoredArea.Name = value;
                _autoSuggestion.ProvidedLocationQuery = value;
            }
        }

        public double GeocircleRadius
        {
            get { return _monitoredArea.Radius; }
            set { _monitoredArea.Radius = value; }
        }

        public bool IsMapLoaded { get; private set; }

        public BitmapImage MapScreenshot
        {
            get { return _selectedAlarm.MapScreen; }
            set { _selectedAlarm.MapScreen = value; }
        }

        public double MaxGeocircleRadius { get; } = 5000;

        public double MinGeocircleRadius { get; } = 100;

        public bool PushpinVisible { get; private set; }

        public ObservableCollection<ReadableLocationName> SuggestedLocations => _autoSuggestion.SuggestedLocations;

        public ICommand SuggestionChosenCommand { get; private set; }

        public ICommand TextChangeCommand { get; private set; }

        public double ZoomLevel
        {
            get; private set;
        }

        public MapViewModel(NavigationServiceWithToken navigationService, GeolocationModel geolocationModel) : base(navigationService)
        {
            _autoSuggestion = new LocationAutoSuggestion(geolocationModel);
            _geolocationModel = geolocationModel;

            TextChangeCommand = _autoSuggestion.TextChangedCommand;
            SuggestionChosenCommand = _autoSuggestion.SuggestionChosenCommand;
        }

        public override void GoBack()
        {
            if (_navigationService.LastPageKey == nameof(AlarmSettingsPage))
                _selectedAlarm.MonitoredArea = _monitoredAreaCopy;
            base.GoBack();
        }

        public override void OnNavigatedFrom(NavigationMessage parameter)
        {
            IsMapLoaded = false;

            MessengerInstance.Unregister<MapLocation>(this, OnSuggestionSelected);
        }

        public override async void OnNavigatedTo(NavigationMessage parameter)
        {
            _monitoredArea = _selectedAlarm.MonitoredArea;
            _monitoredAreaCopy = new MonitoredArea(_selectedAlarm.MonitoredArea);

            MessengerInstance.Register<MapLocation>(this, OnSuggestionSelected);

            IsMapLoaded = false;
            await UpdateUserLocationAsync(ActualLocation).ConfigureAwait(true);
        }

        private async void OnSuggestionSelected(MapLocation selectedLocation)
        {
            Messenger.Default.Send(new MessageBase(), Token.FocusOnMap);
            ActualLocation = selectedLocation.Point;
            await UpdateUserLocationAsync(selectedLocation.Point).ConfigureAwait(true);
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            await TakeMapScreenshotAsync().ConfigureAwait(true);
            _navigationService.NavigateTo(nameof(View.AlarmSettingsPage));
        }

        private async Task TakeMapScreenshotAsync()
        {
            Messenger.Default.Send(new MessageBase(), Token.TakeScreenshot);

            await Task.Factory.StartNew(() =>
            {
                while (MapScreenshot == null)
                {
                }
            }).ConfigureAwait(true);
        }

        [OnCommand("FindMeCommand")]
        private async void UpdatePosition() => await UpdateUserLocationAsync().ConfigureAwait(true);

        private async Task UpdateUserLocationAsync(Geopoint lastKnownPosition = null)
        {
            if (lastKnownPosition == null)
                ActualLocation = (await _geolocationModel.GetActualLocationAsync().ConfigureAwait(true))?.Coordinate?.Point;

            AutoSuggestionLocationQuery = (await _geolocationModel.FindLocationAtAsync(ActualLocation).ConfigureAwait(true))?
                .Take(new[] { 0 })
                .Select(location => new ReadableLocationName(location))
                .First()
                .ToString();
            ZoomLevel = 12;
            PushpinVisible = true;
            MessengerInstance.Send(ActualLocation);
            IsMapLoaded = true;
        }
    }
}