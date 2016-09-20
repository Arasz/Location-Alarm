using System.Collections.Generic;
using System.Linq;

namespace CoreLibrary.Utils
{
    public class SystemSounds
    {
        public static readonly string DefaultSoundUri = "ms-winsoundevent:Notification.Default";

        private static Dictionary<string, string> _sounds = new Dictionary<string, string>
        {
            ["Default"] = "ms-winsoundevent:Notification.Default",
            ["IM"] = "ms-winsoundevent:Notification.IM",
            ["Mail"] = "ms-winsoundevent:Notification.Mail",
            ["Reminder"] = "ms-winsoundevent:Notification.Reminder",
            ["SMS"] = "ms-winsoundevent:Notification.SMS",
        };

        public static IList<string> Names => _sounds.Keys.ToList();

        public static IDictionary<string, string> NameToUriMap => _sounds;

        public static string TrimPrefix(string soundUri) => soundUri.Replace("ms-winsoundevent:Notification.", "");
    }
}