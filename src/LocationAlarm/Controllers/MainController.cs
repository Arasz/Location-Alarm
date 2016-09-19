using CoreLibrary.Data.DataModel.PersistentModel;
using GalaSoft.MvvmLight.Command;
using LocationAlarm.Common;
using LocationAlarm.Controls.AlarmItem;
using LocationAlarm.Model;
using LocationAlarm.Navigation.Bundle;
using LocationAlarm.View;
using LocationAlarm.ViewModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using RelayCommand = GalaSoft.MvvmLight.Command.RelayCommand;

namespace LocationAlarm.Controllers
{
    public class MainController : GenericController<MainPage, MainViewModel, Alarm>
    {
        private readonly AlarmsManager _alarmsManager;

        public MainController(MainPage view, MainViewModel viewModel, Alarm model, AlarmsManager alarmsManager) : base(view, viewModel, model)
        {
            _alarmsManager = alarmsManager;
        }

        public async void AddAlarm()
        {
            _navigationService.NavigateTo(nameof(MapPage), Model, Token.AddNew);
        }

        public override Task InitializeViewAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task InitializeViewModelAsync()
        {
            ViewModel.AddAlarmCommand = new RelayCommand(AddAlarm);
            ViewModel.DeleteAlarmCommand = new RelayCommand<AlarmItemEventArgs>(DeleteAlarm);
            ViewModel.EditAlarmCommand = new RelayCommand<SelectionChangedEventArgs>(EditAlarm);
            ViewModel.LoadDataCommand = new RelayCommand(LoadData);
            ViewModel.ToggleAlarmCommand = new RelayCommand<AlarmItemEventArgs>(ToggleAlarm);
            ViewModel.AlarmsCollection = _alarmsManager.GeolocationAlarms;
            return Task.FromResult(1);
        }

        public async void LoadData()
        {
            await _alarmsManager.ReloadAsync().ConfigureAwait(true);
        }

        public override Task OnDestroyedAsync()
        {
            throw new System.NotImplementedException();
        }

        public override Task OnNavigatedFromAsync(Bundle bundle)
        {
            throw new System.NotImplementedException();
        }

        public override async Task OnNavigatedToAsync(Bundle bundle)
        {
            Model = bundle.Get<Alarm>();

            await ChoseAction().ConfigureAwait(false);
        }

        private async Task ChoseAction()
        {
            switch (_navigationService.LastPageKey)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationService.Token == Token.AddNew)
                        await _alarmsManager.SaveAsync(Model).ConfigureAwait(false);
                    else
                        await _alarmsManager.UpdateAsync(Model).ConfigureAwait(false);
                    break;
            }
        }

        private async void DeleteAlarm(AlarmItemEventArgs eventArgs)
        {
            await _alarmsManager.DeleteAsync(eventArgs.Source).ConfigureAwait(false);
        }

        private void EditAlarm(SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var selectedModel = selectionChangedEventArgs.AddedItems.FirstOrDefault() as Alarm;
            if (selectedModel != null)
                _navigationService.NavigateTo(nameof(AlarmSettingsPage), selectedModel, Token.None);
        }

        private async void ToggleAlarm(AlarmItemEventArgs eventArgs)
        {
            var alarm = eventArgs.Source;
            if (alarm != null)
                await _alarmsManager.ToggleAlarmAsync(alarm).ConfigureAwait(false);
        }
    }
}
}