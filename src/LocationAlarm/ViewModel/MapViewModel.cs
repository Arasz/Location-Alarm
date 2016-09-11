using Commander;
using CoreLibrary.DataModel;
using CoreLibrary.Service.Geolocation;
using CoreLibrary.StateManagement;
using GalaSoft.MvvmLight.Messaging;
using LocationAlarm.Common;
using LocationAlarm.Extensions;
using LocationAlarm.Location.LocationAutosuggestion;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
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
        private readonly IGeolocationService _geolocationService;

        public BasicGeoposition ActualLocation
        {
            get { return CurrentAlarm.Geoposition; }
            private set { CurrentAlarm.Geoposition = value; }
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
            get { return CurrentAlarm.Radius; }
            set { CurrentAlarm.Radius = value; }
        }

        public bool IsMapLoaded { get; private set; }

        public BitmapImage MapScreenshot { get; set; }

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

        public MapViewModel(NavigationServiceWithToken navigationService, IGeolocationService geolocationService) : base(navigationService)
        {
            _autoSuggestion = new LocationAutoSuggestion(geolocationService);
            _geolocationService = geolocationService;

            TextChangeCommand = _autoSuggestion.TextChangedCommand;
            SuggestionChosenCommand = _autoSuggestion.SuggestionChosenCommand;
        }

        public override void GoBack()
        {
            //_selectedAlarm = _navigationService.LastPageKey == nameof(AlarmSettingsPage) ? _monitoredAreaCopy : _monitoredArea;
            AlarmStateManager.Restore();
            _navigationService.GoBack();
        }

        public override void OnNavigatedFrom(object parameter)
        {
            IsMapLoaded = false;
            PushpinVisible = false;

            CurrentAlarm.MapScreenPath = MapScreenshot.UriSource.AbsoluteUri;
            MessengerInstance.Unregister<MapLocation>(this, OnSuggestionSelected);
        }

        public override async void OnNavigatedTo(object parameter)
        {
            CurrentAlarm = parameter as GeolocationAlarm;
            AlarmStateManager = new StateManager<GeolocationAlarm>(CurrentAlarm);
            AlarmStateManager.Save();

            MessengerInstance.Register<MapLocation>(this, OnSuggestionSelected);

            IsMapLoaded = false;

            await UpdateUserLocationAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches location name and geographic position if needed 
        /// </summary>
        private async Task<Tuple<BasicGeoposition, ReadableLocationName>> FetchGeolocationDataFromServiceAsync(bool fetchActualLocation = false)
        {
            BasicGeoposition updatedLocation = ActualLocation;

            if (fetchActualLocation || updatedLocation.IsDefault())
            {
                var geoposition = await _geolocationService.GetActualPositionAsync().ConfigureAwait(false);
                updatedLocation = geoposition.Coordinate.Point.Position;
            }

            var geopoint = new Geopoint(updatedLocation);
            var readableLocationName = await _geolocationService.FindBestMatchedLocationAtAsync(geopoint).ConfigureAwait(false);

            return new Tuple<BasicGeoposition, ReadableLocationName>(updatedLocation, readableLocationName);
        }

        private async void OnSuggestionSelected(MapLocation selectedLocation)
        {
            MessengerInstance.Send(new MessageBase(), Token.FocusOnMap);
            ActualLocation = selectedLocation.Point.Position;

            await UpdateUserLocationAsync().ConfigureAwait(false);
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            MessengerInstance.Send(new MessageBase(), Token.TakeScreenshot);
            MapScreenshot = null;
            await Task.Factory.StartNew(() =>
            {
                while (MapScreenshot == null)
                {
                }
            }).ConfigureAwait(true);

            _navigationService.NavigateTo(nameof(AlarmSettingsPage), CurrentAlarm);
        }

        [OnCommand("FindMeCommand")]
        private async void UpdatePosition() => await UpdateUserLocationAsync(true)
            .ConfigureAwait(false);

        private async Task UpdateUserLocationAsync(bool fetchActualLocation = false)
        {
            // Catch UI thread context
            var locationData = await FetchGeolocationDataFromServiceAsync(fetchActualLocation)
                .ConfigureAwait(true);

            CurrentAlarm.Name = locationData.Item2.ToString();

            ActualLocation = locationData.Item1; // UI
            AutoSuggestionLocationQuery = CurrentAlarm.Name; // UI
            ZoomLevel = 12; // UI
            PushpinVisible = true; // UI
            IsMapLoaded = true; //UI

            MessengerInstance.Send(new Geopoint(ActualLocation));
        }
    }
}