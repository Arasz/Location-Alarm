using Commander;
using CoreLibrary.DataModel;
using CoreLibrary.StateManagement;
using LocationAlarm.Common;
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
        private readonly MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;
        private readonly ResourceLoader _resourceLoader;

        public string AlarmName
        {
            get { return CurrentAlarm.Name; }
            set { CurrentAlarm.Name = value; }
        }

        public IEnumerable<AlarmType> AlarmTypes { get; private set; } = Enum.GetValues(typeof(AlarmType))
            .Cast<AlarmType>().ToList();

        public IEnumerable<WeekDay> DaysOfWeek { get; private set; } = Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>().Select(week => new WeekDay(week)).ToList();

        public BitmapImage MapScreen
        {
            get { return CurrentAlarm.MapScreen; }
            private set { CurrentAlarm.MapScreen = value; }
        }

        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string> { "default" };

        public AlarmType SelectedAlarmType
        {
            get { return CurrentAlarm.AlarmType; }
            set { CurrentAlarm.AlarmType = value; }
        }

        public List<WeekDay> SelectedDays
        {
            get { return CurrentAlarm.ActiveDays; }
            set { CurrentAlarm.ActiveDays = value; }
        }

        public string SelectedNotificationSound
        {
            get { return CurrentAlarm.AlarmSound; }
            set { CurrentAlarm.AlarmSound = value; }
        }

        public AlarmSettingsViewModel(NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _resourceLoader = ResourceLoader.GetForCurrentView("Resources");
            CurrentAlarm = new GeolocationAlarm();
            InitializeAlaram();
        }

        [OnCommand("EditLocationCommand")]
        public void EditLocation()
        {
            _navigationService.NavigateTo(nameof(MapPage), CurrentAlarm);
        }

        public override void GoBack()
        {
            AlarmStateManager.Restore();
            _navigationService.GoBack();
        }

        public override async void OnNavigatedTo(object parameter)
        {
            CurrentAlarm = parameter as GeolocationAlarm;
            AlarmStateManager = new StateManager<GeolocationAlarm>(CurrentAlarm);
            AlarmStateManager.Save();

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
            _navigationService.NavigateTo(nameof(MainPage), CurrentAlarm);
        }

        private void InitializeAlaram()
        {
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