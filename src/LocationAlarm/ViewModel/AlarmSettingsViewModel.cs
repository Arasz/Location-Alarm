using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Navigation;
using PropertyChanged;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class AlarmSettingsViewModel : ViewModelBase, INavigable
    {
        private INavigationService _navigationService;

        public BitmapImage MapScreen { get; set; }

        public AlarmSettingsViewModel()
        {
            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(object parameter)
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            MapScreen = parameter as BitmapImage;
        }
    }
}