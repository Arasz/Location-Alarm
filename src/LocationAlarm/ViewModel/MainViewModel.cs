using Commander;
using GalaSoft.MvvmLight;
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
        private Token _navigationToken;
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
            _navigationToken = Token.AddNew;
            _navigationService.NavigateTo(nameof(MapPage), new NavigationMessage(_navigationService.CurrentPageKey, _alarmsRepository.CreateTransitive(), _navigationToken));
        }

        public void GoBack()
        {
        }

        public void OnNavigatedFrom(NavigationMessage parameter)
        {
        }

        public void OnNavigatedTo(NavigationMessage parameter)
        {
            _navigationToken = parameter.Token;
            switch (parameter.From)
            {
                case nameof(MapPage):
                    break;

                case nameof(AlarmSettingsPage):
                    if (_navigationToken != Token.AddNew) break;
                    var alarm = parameter.Data as AlarmModel;
                    if (alarm == null) return;
                    _alarmsRepository.Add(alarm);
                    break;
            }
        }

        private void EditAlarmExecute(ItemClickEventArgs itemClickEventArgs)
        {
            var clickedItem = itemClickEventArgs.ClickedItem as AlarmModel;
            _navigationService.NavigateTo(nameof(AlarmSettingsPage), new NavigationMessage(_navigationService.CurrentPageKey, clickedItem));
        }
    }
}