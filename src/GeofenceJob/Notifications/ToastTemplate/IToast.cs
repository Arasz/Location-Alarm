using Windows.Data.Xml.Dom;

namespace BackgroundTask.Notifications.ToastTemplate
{
    internal interface IToast
    {
        XmlDocument ToastBlueprint { get; }
    }
}