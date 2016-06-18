using CoreLibrary.DataModel;
using CoreLibrary.StateManagement;
using GalaSoft.MvvmLight;
using LocationAlarm.Navigation;

namespace LocationAlarm.ViewModel
{
    public class ViewModelBaseEx : ViewModelBase, INavigable
    {
        protected readonly NavigationServiceWithToken _navigationService;

        protected StateManager<GeolocationAlarm> AlarmStateManager { get; set; }

        protected GeolocationAlarm CurrentAlarm { get; set; }

        public ViewModelBaseEx(NavigationServiceWithToken navigationService)
        {
            _navigationService = navigationService;
        }

        public virtual void GoBack()
        {
            _navigationService.GoBack();
        }

        public virtual void OnNavigatedFrom(object parameter)
        {
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }
    }
}