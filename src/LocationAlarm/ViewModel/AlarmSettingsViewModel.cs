using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using PropertyChanged;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class AlarmSettingsViewModel : ViewModelBase, INavigable
    {
        /// <summary>
        /// Navigation service 
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Alarm which settings are edited 
        /// </summary>
        private AlarmModel _alarmModel;

        public IEnumerable<string> AlarmTypes { get; }

        public BitmapImage MapScreen { get; private set; }

        public string SelectedAlarmType { get; set; }

        public AlarmSettingsViewModel()
        {
            AlarmTypes = ResourceLoader.GetForCurrentView("Resources").GetString("AlarmSettingsAlarmTypeCollection").Split(',').Select(s => s.Trim());
            SelectedAlarmType = AlarmTypes.First();
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

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings()
        {
            Debug.WriteLine("In On save");
        }
    }
}