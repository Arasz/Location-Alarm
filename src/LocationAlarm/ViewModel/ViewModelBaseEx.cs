using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Common;
using LocationAlarm.Model;
using LocationAlarm.Navigation;

namespace LocationAlarm.ViewModel
{
    public class ViewModelBaseEx : ViewModelBase, INavigable
    {
        protected static AlarmModel _selectedAlarm;
        protected readonly INavigationService _navigationService;
        protected object _lock = new object();
        private static volatile Token _navigationToken;

        public ViewModelBaseEx(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public virtual void GoBack()
        {
            _navigationService.GoBack();
        }

        public virtual void OnNavigatedFrom(NavigationMessage message)
        {
        }

        public virtual void OnNavigatedTo(NavigationMessage message)
        {
        }

        protected Token ReadToken() => _navigationToken;

        protected void SetToken(Token token = Token.None)
        {
            lock (_lock)
            {
                _navigationToken = token;
            }
        }
    }
}