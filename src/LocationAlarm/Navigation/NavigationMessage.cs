using LocationAlarm.Common;

namespace LocationAlarm.Navigation
{
    public class NavigationMessage
    {
        public object Data { get; set; }

        public string From { get; set; }

        public Token Token { get; set; }

        public NavigationMessage(string from, object data, Token token = Token.None)
        {
            From = from;
            Data = data;
            Token = token;
        }
    }
}