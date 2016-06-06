using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    /// <summary>
    /// Extracts readable name from location data 
    /// </summary>
    public interface ILocationNameExtractor
    {
        string Extract(MapLocation location, string userProvidedName);
    }
}