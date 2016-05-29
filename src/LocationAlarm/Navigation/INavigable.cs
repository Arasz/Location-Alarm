namespace LocationAlarm.Navigation
{
    public interface INavigable
    {
        void GoBack();

        void OnNavigatedFrom(object parameter);

        void OnNavigatedTo(object parameter);
    }
}