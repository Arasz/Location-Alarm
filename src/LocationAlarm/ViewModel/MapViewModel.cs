using ArrivalAlarm.Messages;
using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using Microsoft.Practices.ServiceLocation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        private readonly ObservableCollection<string> _foundLocations = new ObservableCollection<string>();

        private MapModel _model;

        /// <summary>
        /// Navigation service used for navigation between pages 
        /// </summary>
        private INavigationService _navigationService;

        /// <summary>
        /// Location name selected in auto suggestion box 
        /// </summary>
        private string _selectedLocation;

        public Geopoint ActualLocation { get; private set; }

        /// <summary>
        /// Text from auto suggest box 
        /// </summary>
        public string AutoSuggestBoxText { get; set; } = "";

        public INotifyCollectionChanged FoundLocations => _foundLocations;

        /// <summary>
        /// Map control screen shoot 
        /// </summary>
        public BitmapImage MapScreenshot { get; set; }

        public bool PushpinVisible { get; private set; }

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

        public MapViewModel()
        {
            _model = new MapModel(TimeSpan.FromMinutes(2));
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            TextChangeCommand = new RelayCommand<bool>(TextChangedCommandExecute);
            SuggestionChosenCommand = new RelayCommand<object>(SuggestionChosenExecute);
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(object parameter)
        {
            MapScreenshot = null;
        }

        public void OnNavigatedTo(object parameter)
        {
            MapScreenshot = null;
            UpdateUserLocation();
        }

        private string GetReadableName(MapLocation location)
        {
            var address = location.Address;

            return $"{address.Town} {address.Street} {address.StreetNumber}";
        }

        [OnCommand("SaveLocationCommand")]
        private async void SaveLocationExecute()
        {
            //TODO: Create real location object
            await TakeMapScreenshotAsync();
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

        private async void SuggestionChosenExecute(object selectedItem)
        {
            if (selectedItem == null)
                return;

            _selectedLocation = (string)selectedItem;
            var locations = await _model.FindLocationAsync(_selectedLocation).ConfigureAwait(true);
            SetProvidedLocation(locations.First());
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private async Task TakeMapScreenshotAsync()
        {
            Messenger.Default.Send(new MapMessage(), Tokens.TakeScreenshot);

            await Task.Factory.StartNew(() =>
            {
                while (MapScreenshot == null) ;
            });
        }

        private async void TextChangedCommandExecute(bool isUserInputReason)
        {
            _foundLocations.Clear();
            if (isUserInputReason && !string.IsNullOrEmpty(AutoSuggestBoxText))
            {
                var userInput = AutoSuggestBoxText;
                var locations = await _model.FindLocationAsync(userInput).ConfigureAwait(true);

                if (locations?.Count == 0)
                    return;

                var foundLocations = locations?.Where(location => GetReadableName(location).Contains(userInput)) ?? new List<MapLocation>();

                foreach (var location in foundLocations)
                {
                    _foundLocations.Add(GetReadableName(location));
                }
            }
        }

        /// <summary>
        /// Updates actual user location 
        /// </summary>
        [OnCommand("FindMeCommand")]
        private async void UpdateUserLocation()
        {
            var actualLocation = await _model.GetActualLocationAsync().ConfigureAwait(true);
            ActualLocation = actualLocation.Coordinate.Point;
            ZoomLevel = 12;
            Messenger.Default.Send(ActualLocation, Tokens.SetMapView);
            PushpinVisible = true;
        }
    }
}