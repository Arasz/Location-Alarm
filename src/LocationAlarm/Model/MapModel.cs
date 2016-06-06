using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace LocationAlarm.Model
{
    public class MapModel
    {
        /// <summary>
        /// Object which contains all logic responsible for localization 
        /// </summary>
        private readonly Geolocator _geolocator;

        /// <summary>
        /// </summary>
        public TimeSpan GetPositionTimeout { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Last known location 
        /// </summary>
        public Geoposition LastKnownLocation { get; private set; }

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

        public MapModel()
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

        /// <summary>
        /// Finds location on map given location name 
        /// </summary>
        /// <param name="locationQuery"> Searched location name </param>
        /// <returns> List of locations matched with given location name </returns>
        public async Task<IReadOnlyList<MapLocation>> FindLocationAsync(string locationQuery, uint maxResultsCount = 6)
        {
            // Give as a hint actual user location because we don't have any informations about
            // searched location
            if (LastKnownLocation == null || (LastLocationFetchTime - DateTime.Now) >= LocationFetchInterval)
                LastKnownLocation = await GetActualLocationAsync().ConfigureAwait(false);

            // Find given location
            var mapLocationFinderResult = await MapLocationFinder.FindLocationsAsync(locationQuery, LastKnownLocation.Coordinate.Point, maxResultsCount);

            if (mapLocationFinderResult.Status == MapLocationFinderStatus.Success &&
                mapLocationFinderResult.Locations.Any())
                return mapLocationFinderResult.Locations;

            return null;
        }

        /// <summary>
        /// Gets actual position on map from GPS 
        /// </summary>
        /// <returns> Actual position on map </returns>
        public async Task<Geoposition> GetActualLocationAsync()
        {
            LastLocationFetchTime = DateTime.Now;

            return await _geolocator.GetGeopositionAsync(LocationFetchInterval, GetPositionTimeout);
        }

        private void PositionChangedHandler(Geolocator sender, PositionChangedEventArgs args)
        {
            LastLocationFetchTime = DateTime.Now;
            LastKnownLocation = args.Position;
        }

        private void StatusChangedHandler(Geolocator sender, StatusChangedEventArgs args)
        {
        }
    }
}