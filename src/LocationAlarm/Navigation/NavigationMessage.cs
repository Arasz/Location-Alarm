namespace LocationAlarm.Navigation
{
    public class NavigationMessage
    {
        public object Data { get; set; }

        public string From { get; set; }

        public NavigationMessage(string from, object data = null)
        {
            From = from;
            Data = data;
        }

        public NavigationMessage()
        {
        }
    }
}