using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.Repository;
using LocationAlarm.View;
using PropertyChanged;
using System.Collections.Specialized;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to. 
    /// <para> Use the <strong> mvvminpc </strong> snippet to add bindable properties to this ViewModel. </para>
    /// <para> You can also use Blend to data bind with the tool's support. </para>
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBase, INavigable
    {
        private readonly AlarmsRepository _alarmsRepository;
        private readonly INavigationService _navigationService;
        public INotifyCollectionChanged AlarmsCollection => _alarmsRepository.Collection;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel(AlarmsRepository alarmsRepository, INavigationService navigationService)
        {
            _alarmsRepository = alarmsRepository;
            _navigationService = navigationService;

            EditAlarmCommand = new RelayCommand<ItemClickEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            _navigationService.NavigateTo(nameof(MapPage), _alarmsRepository.CreateTransitive());
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
            var alarm = parameter as AlarmModel;
            if (alarm == null) return;
            _alarmsRepository.Add(alarm);
        }

        private void EditAlarmExecute(ItemClickEventArgs itemClickEventArgs)
        {
            var clickedItem = itemClickEventArgs.ClickedItem as AlarmModel;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage), clickedItem);
        }
    }
}