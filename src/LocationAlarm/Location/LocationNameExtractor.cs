using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    /// <summary>
    /// Extracts readable location name 
    /// </summary>
    internal class LocationNameExtractor : ILocationNameExtractor
    {
        public virtual string Extract(MapLocation location, string userProvidedName) => Analize(location, userProvidedName).Trim(' ', ',');

        private string Analize(MapLocation location, string userProvidedName)
        {
            var address = location.Address;

            if (!string.IsNullOrEmpty(address.StreetNumber))
                return $"{address.Town}, {address.Street}, {address.StreetNumber}";

            return $"{address.Town}, {address.Street}";
        }
    }
}