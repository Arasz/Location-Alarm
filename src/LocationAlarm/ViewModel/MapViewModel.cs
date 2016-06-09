using ArrivalAlarm.Messages;
using ArrivalAlarm.Model;
using AsyncEventHandler;
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
using System.Linq;
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

        private AlarmModel _alarmModel;
        private LocationAutoSuggestion _autoSuggestion;
        private MonitoredArea _monitoredArea;

        public event AsyncEventHandler<Geopoint> CurrentLocationLoaded;

        public Geopoint ActualLocation
        {
            get { return _monitoredArea.Geopoint; }
            private set { _monitoredArea.Geopoint = value; }
        }

        /// <summary>
        /// Radius of geocircle 
        /// </summary>
        public double GeocircleRadius
        {
            get { return _monitoredArea.Radius; }
            set { _monitoredArea.Radius = value; }
        }

        /// <summary>
        /// State of map loading 
        /// </summary>
        public bool IsMapLoaded { get; private set; }

        /// <summary>
        /// Text from auto suggest box 
        /// </summary>
        public string LocationQuery
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

        /// <summary>
        /// Map control screen shoot 
        /// </summary>
        public BitmapImage MapScreenshot
        {
            get { return _alarmModel.MapScreen; }
            set { _alarmModel.MapScreen = value; }
        }

        public double MaxGeocircleRadius { get; } = 5000;

        public double MinGeocircleRadius { get; } = 100;

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

            _autoSuggestion.SuggestionSelected += OnSuggestionSelected;

            TextChangeCommand = _autoSuggestion.TextChangedCommand;
            SuggestionChosenCommand = _autoSuggestion.SuggestionChosenCommand;

            Messenger.Default.Register<bool>(this, Tokens.MapLoaded, (isMapLoaded) => IsMapLoaded = isMapLoaded);
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(NavigationMessage parameter)
        {
            IsMapLoaded = false;
            //TODO: Check if is called after go back
        }

        public async void OnNavigatedTo(NavigationMessage parameter)
        {
            var model = parameter.Data as AlarmModel;
            if (model == null) return;

            _alarmModel = model;
            _monitoredArea = model.MonitoredArea;

            IsMapLoaded = false;
            MapScreenshot = null;
            await UpdateUserLocationAsync(ActualLocation).ConfigureAwait(true);
        }

        protected virtual async Task OnCurrentLocationLoadedAsync(Geopoint actualLocation)
        {
            var handler = CurrentLocationLoaded;

            if (handler == null) return;

            var invocationList = handler.GetInvocationList().Cast<AsyncEventHandler<Geopoint>>();

            var handlerTasks = invocationList.Select(invocation => invocation.Invoke(this, actualLocation));

            await Task.WhenAll(handlerTasks).ConfigureAwait(true);
        }

        private async void OnSuggestionSelected(object sender, MapLocation selectedLocation)
        {
            Messenger.Default.Send(new MapMessage(), Tokens.FocusOnMap);
            ActualLocation = selectedLocation.Point;
            await OnCurrentLocationLoadedAsync(selectedLocation?.Point).ConfigureAwait(true);
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            await TakeMapScreenshotAsync().ConfigureAwait(true);
            _navigationService.NavigateTo(nameof(View.AlarmSettingsPage), _alarmModel);
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

        [OnCommand("FindMeCommand")]
        private async void UpdatePosition()
        {
            await UpdateUserLocationAsync().ConfigureAwait(true);
        }

        /// <summary>
        /// Updates actual user location 
        /// </summary>

        private async Task UpdateUserLocationAsync(Geopoint lastKnownPosition = null)
        {
            if (lastKnownPosition == null)
                ActualLocation = (await _mapModel.GetActualLocationAsync().ConfigureAwait(true))?.Coordinate?.Point;

            LocationQuery = (await _mapModel.FindLocationAtAsync(ActualLocation).ConfigureAwait(true))?
                .Take(new[] { 0 })
                .Select(location => new ReadableLocationName(location))
                .First()
                .ToString();
            ZoomLevel = 12;
            PushpinVisible = true;
            await OnCurrentLocationLoadedAsync(ActualLocation).ConfigureAwait(true);
        }
    }
}