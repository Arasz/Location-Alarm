using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace CoreLibrary.Service.Geolocation
{
    public class GeolocationService : IGeolocationService
    {
        private readonly Geolocator _geolocator;

        public TimeSpan GetPositionTimeout { get; set; } = TimeSpan.FromSeconds(6);

        public IReadOnlyList<MapLocation> LastFetchedLocations { get; private set; }

        public Geoposition LastFetchedPosition { get; private set; }

        public DateTime LastLocationFetchTime { get; private set; }

        public TimeSpan LocationFetchInterval { get; set; } = TimeSpan.FromMinutes(1);

        public double MovementThreshold { get; set; } = 100;

        public uint ReportInterval { get; set; } = 2000;

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

        public async Task<ReadableLocationName> FindBestMatchedLocationAtAsync(Geopoint queryPoint, Func<MapLocation, bool> matchCriteria = null)
        {
            if (queryPoint == null) throw new ArgumentNullException();

            var locations = await FindLocationAtAsync(queryPoint).ConfigureAwait(false);

            if (locations == null || !locations.Any()) return null;

            return matchCriteria == null ?
                new ReadableLocationName(locations.First()) :
                new ReadableLocationName(locations.First(matchCriteria));
        }

        public async Task<IReadOnlyList<MapLocation>> FindLocationAsync(string locationQuery, uint maxResultsCount = 6)
        {
            // Give as a hint actual user location because we don't have any informations about
            // searched location
            if (LastFetchedPosition == null)
                await GetActualPositionAsync().ConfigureAwait(false);

            var geopoint = LastFetchedPosition.Coordinate.Point;

            // Find given location
            var asyncOperation = MapLocationFinder.FindLocationsAsync(locationQuery, geopoint, maxResultsCount);
            var result = await asyncOperation;

            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Any())
                LastFetchedLocations = result.Locations;

            return LastFetchedLocations;
        }

        public async Task<IReadOnlyList<MapLocation>> FindLocationAtAsync(Geopoint queryPoint)
        {
            var asyncOperation = MapLocationFinder.FindLocationsAtAsync(queryPoint);

            var result = await asyncOperation;

            if (result.Status != MapLocationFinderStatus.Success) return null;
            LastFetchedLocations = result.Locations.ToList();
            return LastFetchedLocations;
        }

        public async Task<Geoposition> GetActualPositionAsync()
        {
            LastLocationFetchTime = DateTime.Now;

            var asyncOperation = _geolocator.GetGeopositionAsync(LocationFetchInterval, GetPositionTimeout);

            LastFetchedPosition = await asyncOperation;

            return LastFetchedPosition;
        }

        private void PositionChangedHandler(Geolocator sender, PositionChangedEventArgs args)
        {
            LastLocationFetchTime = DateTime.Now;
            LastFetchedPosition = args.Position;
        }
    }
}