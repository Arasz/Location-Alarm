﻿using CoreLibrary.Service.Geolocation;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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

        /// <summary>
        /// </summary>
        private readonly IGeolocationService _reverseGeolocationServiceService;

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

        public LocationAutoSuggestion(IGeolocationService reverseGeolocationServiceService)
        {
            _reverseGeolocationServiceService = reverseGeolocationServiceService;

            TextChangedCommand = new RelayCommand<bool>(TextChanged);

            SuggestionChosenCommand = new RelayCommand<object>(SuggestionChosen);
        }

        private async void SuggestionChosen(object selectedItem)
        {
            if (selectedItem == null)
                return;

            UserSelectedSuggestion = (ReadableLocationName)selectedItem;
            LocationQueryResults = await _reverseGeolocationServiceService
                .FindLocationAsync(UserSelectedSuggestion.FullLocationName)
                .ConfigureAwait(true);
            Messenger.Default.Send(LocationQueryResults.FirstOrDefault());
        }

        private async void TextChanged(bool isUserInputReason)
        {
            SuggestedLocations.Clear();
            if (!isUserInputReason || string.IsNullOrEmpty(ProvidedLocationQuery)) return;

            var userInput = ProvidedLocationQuery;
            LocationQueryResults = await _reverseGeolocationServiceService
                .FindLocationAsync(userInput)
                .ConfigureAwait(true);

            LocationQueryResults?
                .Select(location => new ReadableLocationName(location))
                .Where(locationName => !string.IsNullOrEmpty(locationName.MainLocationName))
                .ForEach(locationName => SuggestedLocations.Add(locationName));
        }
    }
}