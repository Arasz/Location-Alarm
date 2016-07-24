using Commander;
using CoreLibrary.DataModel;
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
        private readonly LocationAlarmModel _locationAlarmModel;
        public INotifyCollectionChanged AlarmsCollection => _locationAlarmModel.GeolocationAlarms;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        public MainViewModel(LocationAlarmModel locationAlarmModel, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _locationAlarmModel = locationAlarmModel;

            EditAlarmCommand = new RelayCommand<SelectionChangedEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm()
        {
            CurrentAlarm = _locationAlarmModel.NewAlarm;
            _navigationService.NavigateTo(nameof(MapPage), CurrentAlarm, Token.AddNew);
        }

        public override void GoBack()
        {
        }

        public override void OnNavigatedTo(object parameter)
        {
            CurrentAlarm = parameter as GeolocationAlarm;

            switch (_navigationService.LastPageKey)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationService.Token == Token.AddNew)
                        _locationAlarmModel.SaveAsync(CurrentAlarm);
                    else
                        _locationAlarmModel.Update(CurrentAlarm);
                    break;
            }
        }

        [OnCommand("DeleteAlarmCommand")]
        private void DeleteAlarmExecute(AlarmItemEventArgs eventArgs)
        {
            _locationAlarmModel.Delete(eventArgs.Source);
        }

        private void EditAlarmExecute(SelectionChangedEventArgs itemClickEventArgs)
        {
            CurrentAlarm = itemClickEventArgs.AddedItems.First() as GeolocationAlarm;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage), CurrentAlarm, Token.None);
        }
    }
}