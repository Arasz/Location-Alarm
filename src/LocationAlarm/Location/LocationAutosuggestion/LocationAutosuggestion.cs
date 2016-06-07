using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.Services.Maps;

namespace LocationAlarm.Location.LocationAutosuggestion
{
    public class LocationAutoSuggestion
    {
        public ObservableCollection<ReadableLocationName> SuggestedLocations = new ObservableCollection<ReadableLocationName>();

        private readonly IReverseGeolocationQuery _reverseGeolocationQueryService;

        /// <summary>
        /// Triggered when suggestion from suggested locations is selected 
        /// </summary>
        public event EventHandler<MapLocation> SuggestionSelected;

        public IReadOnlyCollection<MapLocation> LocationQueryResults { get; set; }

        /// <summary>
        /// Text from auto suggest box 
        /// </summary>
        public string ProvidedLocationQuery { get; set; } = "";

        public ICommand SuggestionChosenCommand { get; private set; }

        public ICommand TextChangedCommand { get; private set; }

        /// <summary>
        /// Location name selected in auto suggestion box 
        /// </summary>
        public ReadableLocationName UserSelectedSuggestion { get; set; }

        public LocationAutoSuggestion(IReverseGeolocationQuery reverseGeolocationQueryService)
        {
            _reverseGeolocationQueryService = reverseGeolocationQueryService;

            TextChangedCommand = new RelayCommand<bool>(TextChanged);

            SuggestionChosenCommand = new RelayCommand<object>(SuggestionChosen);
        }

        protected virtual void OnSuggestionSelected(MapLocation selectedLocation)
        {
            SuggestionSelected?.Invoke(this, selectedLocation);
        }

        private async void SuggestionChosen(object selectedItem)
        {
            if (selectedItem == null)
                return;

            UserSelectedSuggestion = (ReadableLocationName)selectedItem;
            LocationQueryResults = await _reverseGeolocationQueryService.FindLocationsAsync(UserSelectedSuggestion.FullLocationName).ConfigureAwait(true);
            OnSuggestionSelected(LocationQueryResults.FirstOrDefault());
        }

        private async void TextChanged(bool isUserInputReason)
        {
            SuggestedLocations.Clear();
            if (!isUserInputReason || string.IsNullOrEmpty(ProvidedLocationQuery)) return;

            var userInput = ProvidedLocationQuery;
            LocationQueryResults = await _reverseGeolocationQueryService.FindLocationsAsync(userInput).ConfigureAwait(true);

            LocationQueryResults?
                .OrderByDescending(location => (location.Address.Town + location.Address.Street + location.Address.StreetNumber).Length)
                .Select(location => new ReadableLocationName(location, userInput))
                .Where(locationName => !string.IsNullOrEmpty(locationName.MainLocationName))
                .ForEach(locationName => SuggestedLocations.Add(locationName));
        }
    }
}