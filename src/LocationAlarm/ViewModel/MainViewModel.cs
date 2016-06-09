using Commander;
using GalaSoft.MvvmLight.Command;
using LocationAlarm.Common;
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
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBaseEx
    {
        private readonly AlarmsRepository _alarmsRepository;
        public INotifyCollectionChanged AlarmsCollection => _alarmsRepository.Collection;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        public MainViewModel(AlarmsRepository alarmsRepository, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _alarmsRepository = alarmsRepository;

            EditAlarmCommand = new RelayCommand<ItemClickEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            _navigationService.Token = Token.AddNew;
            _selectedAlarm = _alarmsRepository.CreateTransitive();
            _navigationService.NavigateTo(nameof(MapPage), new NavigationMessage(_navigationService.CurrentPageKey));
        }

        public override void GoBack()
        {
        }

        public override void OnNavigatedTo(NavigationMessage message)
        {
            switch (_navigationService.LastPageKey)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationService.Token != Token.AddNew) break;
                    _alarmsRepository.Add(_selectedAlarm);
                    break;
            }
        }

        private void EditAlarmExecute(ItemClickEventArgs itemClickEventArgs)
        {
            _selectedAlarm = itemClickEventArgs.ClickedItem as AlarmModel;
            _navigationService.Token = Token.None;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage));
        }
    }
}