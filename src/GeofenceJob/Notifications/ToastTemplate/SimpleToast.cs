using System.Text;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTask.Notifications.ToastTemplate
{
    internal sealed class SimpleToast : IToast
    {
        private XmlDocument _toast;
        public string Header { get; }

        public string Message { get; }

        public string Sound { get; }

        public XmlDocument ToastBlueprint
        {
            get
            {
                if (_toast != null)
                    return _toast;

                _toast = new XmlDocument();
                var xmlFile = BuildXml();
                _toast.LoadXml(xmlFile);

                return _toast;
            }
        }

        public SimpleToast(string header, string message)
        {
            Header = header;
            Message = message;
        }

        public SimpleToast(string header, string message, string sound) : this(header, message)
        {
            Sound = sound;
        }

        private string BuildXml()
        {
            StringBuilder toastString = new StringBuilder();

            toastString.AppendLine("<toast duration=\"long\">");
            toastString.AppendLine("<visual>");
            toastString.AppendLine($"<binding template=\"{ToastTemplateType.ToastText02}\">");
            toastString.AppendLine($"<text id=\"{1}\"> {Header} </text>");
            toastString.AppendLine($"<text id=\"{2}\"> {Message} </text>");
            toastString.AppendLine("</binding>");
            toastString.AppendLine("</visual>");

            if (!string.IsNullOrEmpty(Sound))
                toastString.AppendLine($"<audio src=\"{Sound}\"/>");

            toastString.AppendLine("</toast>");
            return toastString.ToString();
        }
    }
}