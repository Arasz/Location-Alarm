using Commander;
using CoreLibrary.DataModel;
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
        public string AlarmName { get; private set; }

        public IEnumerable<AlarmType> AlarmTypes { get; private set; } = Enum.GetValues(typeof(AlarmType))
            .Cast<AlarmType>();

        public IEnumerable<DayOfWeek> DaysOfWeek { get; private set; } = Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>();

        public BitmapImage MapScreen { get; private set; }

        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string> { "default" };

        public AlarmType SelectedAlarmType { get; set; }

        public List<DayOfWeek> SelectedDays { get; set; }

        public string SelectedNotificationSound { get; set; }

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