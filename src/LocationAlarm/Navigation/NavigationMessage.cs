namespace LocationAlarm.Navigation
{
    public class NavigationMessage
    {
        public object Data { get; set; }

        public string From { get; set; }

        public NavigationMessage(string from, object data)
        {
            From = from;
            Data = data;
        }
    }
}