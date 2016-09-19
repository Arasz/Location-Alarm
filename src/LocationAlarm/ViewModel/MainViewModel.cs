using Commander;
using CoreLibrary.Data.DataModel.PersistentModel;
using LocationAlarm.Common;
using LocationAlarm.Controls.AlarmItem;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBaseEx<Alarm>
    {
        private readonly LocationAlarmModel _alarmsManager;

        public ObservableCollection<Alarm> AlarmsCollection => _alarmsManager.GeolocationAlarms;

        public int SelectedAlarm { get; set; }

        public bool WasDataLoaded { get; private set; }

        public MainViewModel(LocationAlarmModel alarmsManager, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _alarmsManager = alarmsManager;
        }

        [OnCommand("AddNewAlarmCommand")]
        public void AddNewAlarm() => _navigationService.NavigateTo(nameof(MapPage), new Alarm(), Token.AddNew);

        public override void GoBack()
        {
        }

        [OnCommand("LoadDataCommand")]
        public async void LoadData()
        {
            if (WasDataLoaded)
                return;
            WasDataLoaded = true;
            await _alarmsManager.ReloadDataAsync().ConfigureAwait(false);
        }

        public override void OnNavigatedTo(object parameter) => ChoseActionAsync(parameter as Alarm).Wait();

        private async Task ChoseActionAsync(Alarm model)
        {
            switch (_navigationService.LastPageKey)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationService.Token == Token.AddNew)
                        await _alarmsManager.SaveAsync(model).ConfigureAwait(false);
                    else
                        await _alarmsManager.UpdateAsync(model).ConfigureAwait(false);
                    break;
            }
        }

        [OnCommand("DeleteAlarmCommand")]
        private async void DeleteAlarmExecute(AlarmItemEventArgs eventArgs) => await _alarmsManager.DeleteAsync(eventArgs.Source).ConfigureAwait(false);

        [OnCommand("EditAlarmCommand")]
        private void EditAlarmExecute(SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var selectedModel = selectionChangedEventArgs.AddedItems.FirstOrDefault() as Alarm;
            if (selectedModel != null)
                _navigationService.NavigateTo(nameof(AlarmSettingsPage), selectedModel, Token.None);
        }

        [OnCommand("AlarmEnabledChangedCommand")]
        private async void ToggleAlarm(AlarmItemEventArgs eventArgs)
        {
            var alarm = eventArgs.Source;
            if (alarm != null)
                await _alarmsManager.ToggleAlarmAsync(alarm).ConfigureAwait(false);
        }
    }
}