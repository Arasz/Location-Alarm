using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace CoreLibrary.Service.Geolocation
{
    /// <summary>
    /// Monitors actual device geographic position 
    /// </summary>
    public interface IGeolocationService : IReverseGeolocationService
    {
        TimeSpan GetPositionTimeout { get; set; }

        IReadOnlyList<MapLocation> LastFetchedLocations { get; }

        Geoposition LastFetchedPosition { get; }

        DateTime LastLocationFetchTime { get; }

        /// <summary>
        /// Time after which location data will be discarded 
        /// </summary>
        TimeSpan LocationFetchInterval { get; set; }

        /// <summary>
        /// Distance after which position changed event will be raised 
        /// </summary>
        double MovementThreshold { get; set; }

        /// <summary>
        /// Minimum time interval between location updates in milliseconds 
        /// </summary>
        uint ReportInterval { get; set; }

        /// <summary>
        /// Finds locations matching given query 
        /// </summary>
        /// <param name="locationQuery"> Location query </param>
        /// <param name="maxResultsCount"> Maximum number of returned results </param>
        /// <returns></returns>
        Task<IReadOnlyList<MapLocation>> FindLocationAsync(string locationQuery, uint maxResultsCount = 6);

        /// <summary>
        /// Fetches actual device position 
        /// </summary>
        /// <returns></returns>
        Task<Geoposition> GetActualPositionAsync();
    }
}