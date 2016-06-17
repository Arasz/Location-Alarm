using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    public class GeolocationService : IReverseGeolocationQuery
    {
        /// <summary>
        /// Object which contains all logic responsible for localization 
        /// </summary>
        private readonly Geolocator _geolocator;

        /// <summary>
        /// </summary>
        public TimeSpan GetPositionTimeout { get; set; } = TimeSpan.FromSeconds(6);

        public IReadOnlyList<MapLocation> LastFetchedLocations { get; private set; }

        public Geoposition LastFetchedPosition { get; private set; }

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

        public GeolocationService()
        {
            _geolocator = new Geolocator()
            {
                DesiredAccuracy = PositionAccuracy.Default,
                MovementThreshold = MovementThreshold,
                ReportInterval = ReportInterval,
            };

            _geolocator.PositionChanged += PositionChangedHandler;
        }

        /// <summary>
        /// Find location witch best matched to given criteria. If no criteria is given get firs element. 
        /// </summary>
        /// <returns> Location name </returns>
        /// <exception cref="ArgumentNullException"> <paramref name=""/> is <see langword="null"/>. </exception>
        public async Task<ReadableLocationName> FindBestMatchedLocationAtAsync(Geopoint queryPoint, Func<MapLocation, bool> matchCriteria = null)
        {
            if (queryPoint == null) throw new ArgumentNullException();

            var locations = await FindLocationAtAsync(queryPoint).ConfigureAwait(false);

            if (locations == null) return null;

            return matchCriteria == null ?
                new ReadableLocationName(locations.First()) :
                new ReadableLocationName(locations.First(matchCriteria));
        }

        /// <summary>
        /// </summary>
        /// <param name="locationQuery"></param>
        /// <param name="maxResultsCount"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<MapLocation>> FindLocationAsync(string locationQuery, uint maxResultsCount = 6)
        {
            // Give as a hint actual user location because we don't have any informations about
            // searched location
            if (LastFetchedPosition == null)
                await GetActualLocationAsync().ConfigureAwait(false);

            var geopoint = LastFetchedPosition.Coordinate.Point;

            // Find given location
            var asyncOperation = MapLocationFinder.FindLocationsAsync(locationQuery, geopoint, maxResultsCount);
            var result = await asyncOperation;

            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Any())
                LastFetchedLocations = result.Locations;

            return LastFetchedLocations;
        }

        /// <summary>
        /// Gets actual position on map from GPS 
        /// </summary>
        /// <returns> Actual position on map </returns>
        public async Task<Geoposition> GetActualLocationAsync()
        {
            LastLocationFetchTime = DateTime.Now;

            var asyncOperation = _geolocator.GetGeopositionAsync(LocationFetchInterval, GetPositionTimeout);

            LastFetchedPosition = await asyncOperation;

            return LastFetchedPosition;
        }

        private async Task<IReadOnlyList<MapLocation>> FindLocationAtAsync(Geopoint queryPoint)
        {
            var asyncOperation = MapLocationFinder.FindLocationsAtAsync(queryPoint);

            var result = await asyncOperation;

            if (result.Status != MapLocationFinderStatus.Success) return null;
            LastFetchedLocations = result.Locations.ToList();
            return LastFetchedLocations;
        }

        private void PositionChangedHandler(Geolocator sender, PositionChangedEventArgs args)
        {
            LastLocationFetchTime = DateTime.Now;
            LastFetchedPosition = args.Position;
        }
    }
}