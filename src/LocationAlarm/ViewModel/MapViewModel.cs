using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace LocationAlarm.ViewModel
{
    public class MapViewModel : ViewModelBase, INavigable
    {
        private readonly ObservableCollection<string> _foundLocations = new ObservableCollection<string>();

        private Geopoint _actualLocation;

        private MapModel _model;

        /// <summary>
        /// Navigation service used for navigation between pages 
        /// </summary>
        private INavigationService _navigationService;

        private bool _pushpinVisible;
        private string _selectedLocation;

        /// <summary>
        /// Actual user location on map 
        /// </summary>
        public Geopoint ActualLocation
        {
            get { return _actualLocation; }
            private set { Set(nameof(ActualLocation), ref _actualLocation, value); }
        }

        /// <summary>
        /// Text from auto suggest box 
        /// </summary>
        public string AutoSuggestBoxText { get; set; } = "";

        /// <summary>
        /// Command launched when user wants update of his current location 
        /// </summary>
        public ICommand FindMeCommand { get; private set; }

        public INotifyCollectionChanged FoundLocations
        {
            get { return _foundLocations; }
        }

        public bool PushpinVisible
        {
            get { return _pushpinVisible; }
            private set { Set(nameof(PushpinVisible), ref _pushpinVisible, value); }
        }

        public ICommand SaveLocationCommand { get; private set; }

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

            FindMeCommand = new RelayCommand(UpdateUserLocation);
            TextChangeCommand = new RelayCommand<bool>(TextChangedCommandExecute);
            SuggestionChosenCommand = new RelayCommand<object>(SuggestionChosenExecute);
            SaveLocationCommand = new RelayCommand(SaveLocationExecute);
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(object parameter)
        {
            //MyProperty = (parameter as string) ?? string.Empty;
        }

        public void OnNavigatedTo(object parameter)
        {
            UpdateUserLocation();
        }

        private string GetReadableName(MapLocation location)
        {
            var address = location.Address;

            return $"{address.Town} {address.Street} {address.StreetNumber}";
        }

        private void SaveLocationExecute()
        {
            //TODO: Create real location object
            object locationData = new object();

            _navigationService.NavigateTo(nameof(View.AlarmSettingsPage), locationData);
        }

        private void SetProvidedLocation(MapLocation location)
        {
            if (location == null)
                return;

            ActualLocation = location.Point;
            ZoomLevel = 12;
            Messenger.Default.Send(ActualLocation, ArrivalAlarm.Messages.Tokens.MapViewToken);
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
        private async void UpdateUserLocation()
        {
            var actualLocation = await _model.GetActualLocationAsync();
            ActualLocation = actualLocation.Coordinate.Point;
            ZoomLevel = 12;
            Messenger.Default.Send(ActualLocation, ArrivalAlarm.Messages.Tokens.MapViewToken);
            PushpinVisible = true;
        }
    }
}