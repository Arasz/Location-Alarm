using Commander;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Service.Geolocation;
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

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MapViewModel : ViewModelBaseEx<Alarm>
    {
        private readonly LocationAutoSuggestion _autoSuggestion;
        private readonly IGeolocationService _geolocationService;

        private volatile Alarm _model;
        public BasicGeoposition ActualLocation { get; set; }

        public string AlarmName { get; set; }

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

        public double GeocircleRadius { get; set; }

        public bool IsMapLoaded { get; private set; }

        public string MapScreenshotPath { get; set; }

        public double MaxGeocircleRadius { get; } = 5000;

        public double MinGeocircleRadius { get; } = 100;

        public bool PushpinVisible { get; private set; }

        public ObservableCollection<ReadableLocationName> SuggestedLocations => _autoSuggestion.SuggestedLocations;

        public ICommand SuggestionChosenCommand { get; private set; }

        public ICommand TextChangeCommand { get; private set; }

        public double ZoomLevel { get; private set; }

        public MapViewModel(NavigationServiceWithToken navigationService, IGeolocationService geolocationService) : base(navigationService)
        {
            _autoSuggestion = new LocationAutoSuggestion(geolocationService);
            _geolocationService = geolocationService;

            TextChangeCommand = _autoSuggestion.TextChangedCommand;
            SuggestionChosenCommand = _autoSuggestion.SuggestionChosenCommand;
        }

        public override void GoBack() => _navigationService.GoBack();

        public override void OnNavigatedFrom(object parameter)
        {
            IsMapLoaded = false;
            PushpinVisible = false;

            MessengerInstance.Unregister<MapLocation>(this, OnSuggestionSelected);
        }

        public override async void OnNavigatedTo(object parameter)
        {
            _model = (parameter as Alarm);
            InitializeViewModel(_model);

            MessengerInstance.Register<MapLocation>(this, OnSuggestionSelected);

            IsMapLoaded = false;

            await UpdateUserLocationAsync().ConfigureAwait(false);
        }

        protected override void InitializeViewModel(Alarm dataSource)
        {
            AlarmName = dataSource.Name;
            AutoSuggestionLocationQuery = AlarmName;
            GeocircleRadius = dataSource.Radius;
            ActualLocation = new BasicGeoposition
            {
                Altitude = dataSource.Altitude,
                Longitude = dataSource.Longitude,
                Latitude = dataSource.Latitude
            };
            MapScreenshotPath = dataSource.MapScreenPath;
        }

        protected override Alarm SaveDataToModel(Alarm prototype)
        {
            var newModel = new Alarm
            {
                Id = prototype.Id,
                AlarmSound = prototype.AlarmSound,
                AlarmType = prototype.AlarmType,
                IsActive = prototype.IsActive,
                ActiveDays = prototype.ActiveDays,

                Name = AlarmName,
                Altitude = ActualLocation.Altitude,
                Latitude = ActualLocation.Latitude,
                Longitude = ActualLocation.Longitude,
                MapScreenPath = MapScreenshotPath,
                Radius = GeocircleRadius,
            };
            return newModel;
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
            MapScreenshotPath = null;
            await Task.Run(() =>
            {
                while (MapScreenshotPath == null)
                {
                }
            }).ConfigureAwait(true);

            _navigationService.NavigateTo(nameof(AlarmSettingsPage), SaveDataToModel(_model));
        }

        [OnCommand("FindMeCommand")]
        private async void UpdatePosition() => await UpdateUserLocationAsync(true)
            .ConfigureAwait(false);

        private async Task UpdateUserLocationAsync(bool fetchActualLocation = false)
        {
            // Catch UI thread context
            var locationData = await FetchGeolocationDataFromServiceAsync(fetchActualLocation)
                .ConfigureAwait(true);

            AlarmName = locationData.Item2.ToString();

            ActualLocation = locationData.Item1; // UI
            AutoSuggestionLocationQuery = AlarmName; // UI
            ZoomLevel = 12; // UI
            PushpinVisible = true; // UI
            IsMapLoaded = true; //UI

            MessengerInstance.Send(new Geopoint(ActualLocation));
        }
    }
}