using CoreLibrary.Service.Geolocation;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace LocationAlarm.DeviceAccess
{
    public class AccessManager : IAccessManager
    {
        private readonly IGeolocationService _geolocationService;
        public DeviceAccessInformation AccessInformation { get; }

        public AccessManager(IGeolocationService _geolocationService)
        {
            this._geolocationService = _geolocationService;
            AccessInformation = DeviceAccessInformation.CreateFromDeviceClass(DeviceClass.Location);
            AccessInformation.AccessChanged += AccessInformationOnAccessChanged;
        }

        /// <summary>
        /// Asks user for all permissions 
        /// </summary>
        /// <returns> True if all granted </returns>
        public async Task<bool> AskForPermissionsAsync()
        {
            switch (AccessInformation.CurrentStatus)
            {
                case DeviceAccessStatus.Unspecified:
                    return false;

                case DeviceAccessStatus.Allowed:
                    return true;

                case DeviceAccessStatus.DeniedByUser:
                    return await TryGetPermissionsAsync().ConfigureAwait(false);

                case DeviceAccessStatus.DeniedBySystem:
                    return false;

                default:
                    return false;
            }
        }

        private async void AccessInformationOnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs args)
        {
            if (args.Status == DeviceAccessStatus.DeniedByUser)
                await TryGetPermissionsAsync().ConfigureAwait(false);
        }

        private async Task<bool> TryGetPermissionsAsync()
        {
            await _geolocationService.GetActualPositionAsync().ConfigureAwait(false);
            return AccessInformation.CurrentStatus == DeviceAccessStatus.Allowed;
        }
    }
}