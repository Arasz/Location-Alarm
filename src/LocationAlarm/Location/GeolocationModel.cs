using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    public class GeolocationModel : IReverseGeolocationQuery
    {
        /// <summary>
        /// Object which contains all logic responsible for localization 
        /// </summary>
        private readonly Geolocator _geolocator;

        private volatile bool _undergoingFindLocationAtQuery;
        private volatile bool _undergoingFindLocationQuery;
        private volatile bool _undergoingLocationQuery;

        /// <summary>
        /// </summary>
        public TimeSpan GetPositionTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public IReadOnlyList<MapLocation> LastKnownLocations { get; private set; }

        /// <summary>
        /// Last known location 
        /// </summary>
        public Geoposition LastKnownPosition { get; private set; }

        /// <summary>
        /// Time when location data was retrieved 
        /// </summary>
        public DateTime LastLocationFetchTime { get; private set; }

        /// <summary>
        /// Time after which location data will be discarded 
        /// </summary>
        public TimeSpan LocationFetchInterval { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Distance after which position changed event will be raised 
        /// </summary>
        public double MovementThreshold { get; set; } = 500;

        /// <summary>
        /// Minimum time interval between location updates in milliseconds 
        /// </summary>
        public uint ReportInterval { get; set; } = 1000;

        public GeolocationModel()
        {
            _geolocator = new Geolocator()
            {
                DesiredAccuracy = PositionAccuracy.Default,
                MovementThreshold = MovementThreshold,
                ReportInterval = ReportInterval,
            };

            _geolocator.PositionChanged += PositionChangedHandler;
            _geolocator.StatusChanged += StatusChangedHandler;
        }

        public async Task<IReadOnlyList<MapLocation>> FindLocationAsync(string locationQuery, uint maxResultsCount = 6)
        {
            if (_undergoingFindLocationQuery) return LastKnownLocations;
            _undergoingFindLocationQuery = true;
            // Give as a hint actual user location because we don't have any informations about
            // searched location
            if (LastKnownPosition == null || (LastLocationFetchTime - DateTime.Now) >= LocationFetchInterval)
                await GetActualLocationAsync().ConfigureAwait(false);

            // Find given location
            var result = await MapLocationFinder
                .FindLocationsAsync(locationQuery, LastKnownPosition.Coordinate.Point, maxResultsCount);

            _undergoingFindLocationQuery = false;

            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Any())
                LastKnownLocations = result.Locations;

            return LastKnownLocations;
        }

        public async Task<IReadOnlyList<MapLocation>> FindLocationAtAsync(Geopoint queryPoint = null)
        {
            if (_undergoingFindLocationAtQuery) return LastKnownLocations;
            _undergoingFindLocationAtQuery = true;

            if (queryPoint == null && LastKnownPosition != null)
                queryPoint = LastKnownPosition.Coordinate.Point;

            var result = await MapLocationFinder.FindLocationsAtAsync(queryPoint);

            _undergoingFindLocationAtQuery = false;

            if (result.Status != MapLocationFinderStatus.Success) return LastKnownLocations;
            LastKnownLocations = result.Locations;
            return result.Locations;
        }

        /// <summary>
        /// Gets actual position on map from GPS 
        /// </summary>
        /// <returns> Actual position on map </returns>
        public async Task<Geoposition> GetActualLocationAsync()
        {
            if (_undergoingLocationQuery) return LastKnownPosition;
            _undergoingLocationQuery = true;

            LastLocationFetchTime = DateTime.Now;

            var newLocation = await _geolocator.GetGeopositionAsync(LocationFetchInterval, GetPositionTimeout);
            LastKnownPosition = newLocation;
            _undergoingLocationQuery = false;
            return newLocation;
        }

        private void PositionChangedHandler(Geolocator sender, PositionChangedEventArgs args)
        {
            LastLocationFetchTime = DateTime.Now;
            LastKnownPosition = args.Position;
        }

        private void StatusChangedHandler(Geolocator sender, StatusChangedEventArgs args)
        {
        }
    }
}