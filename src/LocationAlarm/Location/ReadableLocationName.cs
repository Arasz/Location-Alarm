using Windows.Services.Maps;

namespace LocationAlarm.Location
{
    /// <summary>
    /// Readable, adjusted to binding location name <remarks> Doesn't hold reference to input objects </remarks>
    /// </summary>
    public class ReadableLocationName
    {
        public string FullLocationName => (LocationNameDetails + ", " + MainLocationName).Trim(' ', ',');

        public string LocationNameDetails { get; }

        public string MainLocationName { get; }

        public ReadableLocationName(MapLocation location, string userProvidedName = "")
        {
            var address = location.Address;
            MainLocationName = $"{address.Town}, {address.Street}, {address.StreetNumber}".Trim(',', ' ');

            if (string.IsNullOrEmpty(address.Country) && string.IsNullOrEmpty(address.Region)) return;

            LocationNameDetails = $"{address.Country}, {address.Region}".Trim(' ', ',');
        }

        public override string ToString() => MainLocationName;
    }
}