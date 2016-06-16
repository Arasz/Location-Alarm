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
        private readonly LocationAlarmsManager _locationAlarmsManager;
        public INotifyCollectionChanged AlarmsCollection => _locationAlarmsManager.Collection;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        public MainViewModel(LocationAlarmsManager locationAlarmsManager, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _locationAlarmsManager = locationAlarmsManager;

            EditAlarmCommand = new RelayCommand<SelectionChangedEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            _navigationService.Token = Token.AddNew;
            SelectedLocationAlarm = _locationAlarmsManager.CreateTransitive();
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
                    _locationAlarmsManager.Add(SelectedLocationAlarm);
                    break;
            }
        }

        [OnCommand("DeleteAlarmCommand")]
        private void DeleteAlarmExecute(AlarmItemEventArgs eventArgs)
        {
            _locationAlarmsManager.Remove(eventArgs.Source);
        }

        private void EditAlarmExecute(SelectionChangedEventArgs itemClickEventArgs)
        {
            SelectedLocationAlarm = itemClickEventArgs.AddedItems.First() as Model.LocationAlarm;
            _navigationService.Token = Token.None;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage));
        }
    }
}