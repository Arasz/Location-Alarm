using Commander;
using GalaSoft.MvvmLight.Command;
using LocationAlarm.Common;
using LocationAlarm.Controls.AlarmItem;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBaseEx
    {
        private readonly LocationAlarmManager _locationAlarmModel;
        public INotifyCollectionChanged AlarmsCollection => _locationAlarmModel.Collection;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        public MainViewModel(LocationAlarmManager locationAlarmModel, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _locationAlarmModel = locationAlarmModel;

            EditAlarmCommand = new RelayCommand<SelectionChangedEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            _navigationService.Token = Token.AddNew;
            _selectedAlarm = _locationAlarmModel.CreateTransitive();
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
                    _locationAlarmModel.Add(_selectedAlarm);
                    break;
            }
        }

        [OnCommand("DeleteAlarmCommand")]
        private void DeleteAlarmExecute(AlarmItemEventArgs eventArgs)
        {
            _locationAlarmModel.Remove(eventArgs.Source);
        }

        private void EditAlarmExecute(SelectionChangedEventArgs itemClickEventArgs)
        {
            _selectedAlarm = itemClickEventArgs.AddedItems.First() as AlarmModel;
            _navigationService.Token = Token.None;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage));
        }
    }
}