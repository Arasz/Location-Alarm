using ArrivalAlarm.Model;
using Commander;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
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
            PushpinVisible = false;
            MessengerInstance.Unregister<MapLocation>(this, OnSuggestionSelected);
        }

        public override void OnNavigatedTo(NavigationMessage parameter)
        {
            _monitoredArea = _selectedAlarm.MonitoredArea;
            _monitoredAreaCopy = new MonitoredArea(_selectedAlarm.MonitoredArea);

            MessengerInstance.Register<MapLocation>(this, OnSuggestionSelected);

            IsMapLoaded = false;
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await UpdateUserLocationAsync().ConfigureAwait(true);
            });
        }

        private void OnSuggestionSelected(MapLocation selectedLocation)
        {
            Messenger.Default.Send(new MessageBase(), Token.FocusOnMap);
            ActualLocation = selectedLocation.Point;
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                await UpdateUserLocationAsync().ConfigureAwait(true);
            });
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            Messenger.Default.Send(new MessageBase(), Token.TakeScreenshot);
            MapScreenshot = null;
            await Task.Factory.StartNew(() =>
            {
                while (MapScreenshot == null)
                {
                }
            }).ConfigureAwait(true);
            _navigationService.NavigateTo(nameof(AlarmSettingsPage));
        }

        [OnCommand("FindMeCommand")]
        private async void UpdatePosition() => await UpdateUserLocationAsync(true).ConfigureAwait(true);

        private async Task UpdateUserLocationAsync(bool fetchActualLocation = false)
        {
            if (fetchActualLocation || ActualLocation == null)
                ActualLocation = (await _geolocationModel.GetActualLocationAsync().ConfigureAwait(true))?.Coordinate?.Point;
            AutoSuggestionLocationQuery = _geolocationModel.LastKnownLocations?
                .Take(new[] { 0 })
                .Select(location => new ReadableLocationName(location))
                .First()
                .ToString();
            _monitoredArea.Name = AutoSuggestionLocationQuery;
            ZoomLevel = 12;
            PushpinVisible = true;
            MessengerInstance.Send(ActualLocation);
            IsMapLoaded = true;
        }
    }
}