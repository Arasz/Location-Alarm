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
        public TimeSpan GetPositionTimeout { get; set; } = TimeSpan.FromSeconds(6);

        public IReadOnlyList<MapLocation> LastKnownLocations { get; private set; }

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

        /// <summary>
        /// Gets actual position on map from GPS 
        /// </summary>
        /// <returns> Actual position on map </returns>
        public async Task<Geoposition> GetActualLocationAsync()
        {
            //BUG : There is soruce
            if (_undergoingLocationQuery) return LastKnownPosition;
            _undergoingLocationQuery = true;

            LastLocationFetchTime = DateTime.Now;

            LastKnownPosition = await _geolocator.GetGeopositionAsync(LocationFetchInterval, GetPositionTimeout);
            await FindLocationAtAsync().ConfigureAwait(true);

            _undergoingLocationQuery = false;
            return LastKnownPosition;
        }

        private async Task FindLocationAtAsync(Geopoint queryPoint = null)
        {
            if (_undergoingFindLocationAtQuery) return;
            _undergoingFindLocationAtQuery = true;

            if (queryPoint == null && LastKnownPosition != null)
                queryPoint = LastKnownPosition.Coordinate.Point;

            var result = await MapLocationFinder.FindLocationsAtAsync(queryPoint);

            _undergoingFindLocationAtQuery = false;

            if (result.Status != MapLocationFinderStatus.Success) return;
            LastKnownLocations = result.Locations;
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