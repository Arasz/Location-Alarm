using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace LocationAlarm.DeviceAccess
{
    /// <summary>
    /// Provides interface for asking for access to necessary devices 
    /// </summary>
    public interface IAccessManager
    {
        DeviceAccessInformation AccessInformation { get; }

        Task<bool> AskForPermissionsAsync();
    }
}