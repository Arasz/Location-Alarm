using Commander;
using LocationAlarm.Common;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.Utils;
using LocationAlarm.View;
using Microsoft.Practices.ServiceLocation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class AlarmSettingsViewModel : ViewModelBaseEx
    {
        private readonly Dictionary<string, AlarmType> _alarmTypeMap = new Dictionary<string, AlarmType>();
        private readonly MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;
        private readonly ResourceLoader _resourceLoader;
        private string _selectedDaysConcated;
        public string AlarmName => _selectedAlarm.MonitoredArea.Name;

        public IEnumerable<AlarmType> AlarmTypes { get; private set; } = Enum.GetValues(typeof(AlarmType)).Cast<AlarmType>();

        public IEnumerable<DayOfWeek> DaysOfWeek { get; private set; } = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>();

        public BitmapImage MapScreen => _selectedAlarm.MapScreen;

        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string> { "default" };

        public string SelectedAlarmType
        {
            get { return _resourceLoader.GetString(_selectedAlarm.AlarmType.ToString()); }
            set { _selectedAlarm.AlarmType = _alarmTypeMap[value]; }
        }

        public ISet<DayOfWeek> SelectedDays
        {
            get { return _selectedAlarm.ActiveDays; }
            set
            {
                _selectedAlarm.ActiveDays = value;
            }
        }

        public string SelectedNotificationSound
        {
            get { return _selectedAlarm.AlarmSound; }
            set { _selectedAlarm.AlarmSound = value; }
        }

        public AlarmSettingsViewModel(NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

            InitializeAlaram();
        }

        [OnCommand("EditLocationCommand")]
        public void EditLocation()
        {
            _navigationService.NavigateTo(nameof(MapPage));
        }

        public override async void OnNavigatedTo(NavigationMessage message)
        {
            if (NotificationSounds.Count() <= 1)
                await InitializeSoundFileNamesAsync().ConfigureAwait(true);
            if (_navigationService.Token == Token.AddNew)
                InitializeAlaram();
        }

        [OnCommand("PlaySoundCommand")]
        public void OnPlaySound(string soundName)
        {
            _mediaPlayer.SetUriSource(new Uri(_resourceLoader.GetString("SoundFileAppendix") + soundName + ".mp3"));
            _mediaPlayer.Play();
        }

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings()
        {
            _navigationService.NavigateTo(nameof(MainPage));
        }

        private void InitializeAlaram()
        {
            SelectedAlarmType = _resourceLoader.GetString(AlarmTypes.First().ToString());
            SelectedNotificationSound = NotificationSounds.First();
        }

        private async Task InitializeSoundFileNamesAsync()
        {
            var notificationSounds = await ServiceLocator.Current
                .GetInstance<IAssetsNamesReader>()
                .ReadAsync("Sounds").ConfigureAwait(true);

            NotificationSounds = notificationSounds
                .Select(s => s.Replace(".mp3", ""));
        }
    }
}