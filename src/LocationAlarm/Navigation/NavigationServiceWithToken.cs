using GalaSoft.MvvmLight.Views;
using LocationAlarm.Common;

namespace LocationAlarm.Navigation
{
    public class NavigationServiceWithToken
    {
        private readonly object _lock = new object();
        private readonly INavigationService _navigationService;
        private volatile Token _token;
        public string CurrentPageKey => _navigationService.CurrentPageKey;

        public string LastPageKey { get; set; }

        public Token Token
        {
            get { return _token; }
            set
            {
                lock (_lock)
                    _token = value;
            }
        }

        public NavigationServiceWithToken(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void NavigateTo(string pageKey)
        {
            LastPageKey = CurrentPageKey;
            _navigationService.NavigateTo(pageKey);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            LastPageKey = CurrentPageKey;
            _navigationService.NavigateTo(pageKey, parameter);
        }
    }
}