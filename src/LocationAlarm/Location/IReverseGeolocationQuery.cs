using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    /// <summary>
    /// Can query location service for locations matching given query 
    /// </summary>
    public interface IReverseGeolocationQuery
    {
        /// <summary>
        /// Finds locations matching given query 
        /// </summary>
        /// <param name="locationQuery"> Location query </param>
        /// <param name="maxResultsCount"> Maximum number of returned results </param>
        /// <returns></returns>
        Task<IReadOnlyList<MapLocation>> FindLocationsAsync(string locationQuery, uint maxResultsCount = 6);
    }
}