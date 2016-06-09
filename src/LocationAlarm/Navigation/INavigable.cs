namespace LocationAlarm.Navigation
{
    public interface INavigable
    {
        void GoBack();

        void OnNavigatedFrom(NavigationMessage message);

        void OnNavigatedTo(NavigationMessage message);
    }
}