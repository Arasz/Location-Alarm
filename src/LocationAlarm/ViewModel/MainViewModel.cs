using CoreLibrary.Data.DataModel.PersistentModel;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class MainViewModel : ViewModelBaseEx<Alarm>
    {
        public ICommand AddAlarmCommand { get; set; }

        public ObservableCollection<Alarm> AlarmsCollection { get; set; }

        public ICommand DeleteAlarmCommand { get; set; }

        public ICommand EditAlarmCommand { get; set; }

        public ICommand LoadDataCommand { get; set; }

        public int SelectedAlarm { get; set; }

        public ICommand ToggleAlarmCommand { get; set; }

        public bool WasDataLoaded { get; set; }

        public MainViewModel(AlarmsManager alarmsManager, NavigationServiceWithToken navigationService) : base(navigationService)
        {
        }
    }