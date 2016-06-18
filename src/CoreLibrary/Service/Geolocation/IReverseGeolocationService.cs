using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace CoreLibrary.Service.Geolocation
{
    /// <summary>
    /// Can query location service for locations matching given geographic point 
    /// </summary>
    public interface IReverseGeolocationService
    {
        /// <summary>
        /// Finds location best matched given geographic point and match criteria 
        /// </summary>
        /// <returns></returns>
        Task<ReadableLocationName> FindBestMatchedLocationAtAsync(Geopoint queryPoint, Func<MapLocation, bool> matchCriteria = null);

        /// <summary>
        /// Finds locations matching given geographic point 
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<MapLocation>> FindLocationAtAsync(Geopoint queryPoint);
    }
}