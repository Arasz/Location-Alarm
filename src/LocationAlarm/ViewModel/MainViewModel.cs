using Commander;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
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

        public MainViewModel(AlarmsRepository alarmsRepository, INavigationService navigationService) : base(navigationService)
        {
            _alarmsRepository = alarmsRepository;

            EditAlarmCommand = new RelayCommand<ItemClickEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            SetToken(Token.AddNew);
            _selectedAlarm = _alarmsRepository.CreateTransitive();
            _navigationService.NavigateTo(nameof(MapPage), new NavigationMessage(_navigationService.CurrentPageKey));
        }

        public override void GoBack()
        {
        }

        public override void OnNavigatedTo(NavigationMessage message)
        {
            if (message == null) return;
            switch (message.From)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (ReadToken() != Token.AddNew) break;
                    _alarmsRepository.Add(_selectedAlarm);
                    break;
            }
        }

        private void EditAlarmExecute(ItemClickEventArgs itemClickEventArgs)
        {
            _selectedAlarm = itemClickEventArgs.ClickedItem as AlarmModel;
            SetToken();
            _navigationService.NavigateTo(nameof(AlarmSettingsPage));
        }
    }
}