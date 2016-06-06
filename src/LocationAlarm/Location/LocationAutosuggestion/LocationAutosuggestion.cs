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
        public ObservableCollection<string> SuggestedLocations = new ObservableCollection<string>();

        private readonly ILocationNameExtractor _locationNameExtractor;
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
        public string UserSelectedSuggestion { get; set; }

        public LocationAutoSuggestion(IReverseGeolocationQuery reverseGeolocationQueryService, ILocationNameExtractor extractor)
        {
            _locationNameExtractor = extractor;
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

            UserSelectedSuggestion = (string)selectedItem;
            LocationQueryResults = await _reverseGeolocationQueryService.FindLocationsAsync(UserSelectedSuggestion).ConfigureAwait(true);
            OnSuggestionSelected(LocationQueryResults.FirstOrDefault());
        }

        private async void TextChanged(bool isUserInputReason)
        {
            SuggestedLocations.Clear();
            if (!isUserInputReason || string.IsNullOrEmpty(ProvidedLocationQuery)) return;

            var userInput = ProvidedLocationQuery;
            LocationQueryResults = await _reverseGeolocationQueryService.FindLocationsAsync(userInput).ConfigureAwait(true);

            if (LocationQueryResults == null) return;

            foreach (var location in LocationQueryResults)
            {
                var readableName = _locationNameExtractor.Extract(location, userInput);
                if (!string.IsNullOrEmpty(readableName))
                    SuggestedLocations.Add(readableName);
            }
            SuggestedLocations.OrderByDescending(s => s.Length);
        }
    }
}