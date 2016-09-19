using Commander;
using CoreLibrary.Data.DataModel.PersistentModel;
using GalaSoft.MvvmLight.Command;
using LocationAlarm.Common;
using LocationAlarm.Controls.AlarmItem;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBaseEx<Alarm>
    {
        private readonly LocationAlarmModel _locationAlarmModel;

        public INotifyCollectionChanged AlarmsCollection => _locationAlarmModel.GeolocationAlarms;

        public ICommand EditAlarmCommand { get; private set; }

        public int SelectedAlarm { get; set; }

        public bool WasDataLoaded { get; private set; }

        public MainViewModel(LocationAlarmModel locationAlarmModel, NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _locationAlarmModel = locationAlarmModel;

            EditAlarmCommand = new RelayCommand<SelectionChangedEventArgs>(EditAlarmExecute);
        }

        [OnCommand("AddNewAlarmCommand")]
        public async void AddNewAlarm()
        {
            Model = await CreateModelAsync().ConfigureAwait(true);
            _navigationService.NavigateTo(nameof(MapPage), Model, Token.AddNew);
        }

        public override void GoBack()
        {
        }

        [OnCommand("LoadDataCommand")]
        public async void LoadData()
        {
            if (WasDataLoaded)
                return;
            WasDataLoaded = true;
            await _locationAlarmModel.ReloadDataAsync().ConfigureAwait(true);
        }

        public override async void OnNavigatedTo(object parameter)
        {
            Model = parameter as Alarm;

            switch (_navigationService.LastPageKey)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationService.Token == Token.AddNew)
                        await _locationAlarmModel.SaveAsync(Model).ConfigureAwait(false);
                    else
                        await _locationAlarmModel.UpdateAsync(Model).ConfigureAwait(false);
                    break;
            }
        }

        protected override Task<Alarm> CreateModelAsync() => Task.FromResult(new Alarm());

        protected override Task InitializeFromModelAsync(Alarm model)
        {
            throw new System.NotImplementedException();
        }

        [OnCommand("DeleteAlarmCommand")]
        private async void DeleteAlarmExecute(AlarmItemEventArgs eventArgs)
        {
            await _locationAlarmModel.DeleteAsync(eventArgs.Source).ConfigureAwait(false);
        }

        private void EditAlarmExecute(SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var selectedModel = selectionChangedEventArgs.AddedItems.FirstOrDefault() as Alarm;
            if (selectedModel != null)
                _navigationService.NavigateTo(nameof(AlarmSettingsPage), selectedModel, Token.None);
        }

        [OnCommand("AlarmEnabledChangedCommand")]
        private async void IsAlarmEnabledChangedCommnad(AlarmItemEventArgs eventArgs)
        {
            var alarm = eventArgs.Source;
            if (alarm != null)
                await _locationAlarmModel.ToggleAlarmAsync(alarm).ConfigureAwait(false);
        }
    }
}