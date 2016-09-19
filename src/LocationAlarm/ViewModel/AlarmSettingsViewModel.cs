using Commander;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Utils;
using CoreLibrary.Utils.ScreenshotManager;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class AlarmSettingsViewModel : ViewModelBaseEx<Alarm>
    {
        private readonly IScreenshotManager _screenshotManager;

        public string AlarmName { get; set; }

        public IEnumerable<AlarmType> AlarmTypes { get; private set; } = Enum.GetValues(typeof(AlarmType))
            .Cast<AlarmType>().ToList();

        public IEnumerable<string> DaysOfWeek { get; private set; } = DateTimeFormatInfo.CurrentInfo.DayNames;

        public BitmapImage MapScreen { get; set; }

        public IEnumerable<string> NotificationSounds { get; } = SystemSounds.Names;

        public AlarmType SelectedAlarmType { get; set; }

        public List<string> SelectedDays { get; set; } = new List<string>();

        public string SelectedNotificationSound { get; set; }

        private MediaPlayer _mediaPlayer => BackgroundMediaPlayer.Current;

        public AlarmSettingsViewModel(NavigationServiceWithToken navigationService, IScreenshotManager screenshotManager)
            : base(navigationService)
        {
            _screenshotManager = screenshotManager;

            ResourceLoader.GetForCurrentView("Resources");
        }

        [OnCommand("EditLocationCommand")]
        public void EditLocation()
        {
            var model = CreateModelAsync().Result;
            _navigationService.NavigateTo(nameof(MapPage), model);
        }

        public override void GoBack()
        {
            _navigationService.GoBack();
        }

        [OnCommand("LoadedCommand")]
        public async void Loaded()
        {
            //MapScreen = await _screenshotManager.OpenScreenFromPathAsync(Model.MapScreenPath)
            //    .ConfigureAwait(false);
        }

        public override async void OnNavigatedTo(object parameter)
        {
            Model = parameter as Alarm;

            await InitializeFromModelAsync(parameter as Alarm).ConfigureAwait(false);
        }

        [OnCommand("PlaySoundCommand")]
        public void OnPlaySound(string soundName)
        {
            var uri = new Uri(SystemSounds.NameToUriMap[SelectedNotificationSound], UriKind.RelativeOrAbsolute);
            _mediaPlayer.MediaFailed += MediaPlayerOnMediaFailed;
            _mediaPlayer.SetUriSource(uri);
            _mediaPlayer.Play();
        }

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings()
        {
            var model = CreateModelAsync().Result;
            _navigationService.NavigateTo(nameof(MainPage), model);
        }

        protected override Task<Alarm> CreateModelAsync()
        {
            var newModel = new Alarm
            {
                ActiveDays = ParseSelectedDays(),
                AlarmSound = SystemSounds.NameToUriMap[SelectedNotificationSound],
                AlarmType = SelectedAlarmType,

                Id = Model.Id,
                Name = Model.Name,
                IsActive = Model.IsActive,
                MapScreenPath = Model.MapScreenPath,
                Radius = Model.Radius,
                Latitude = Model.Latitude,
                Longitude = Model.Longitude,
                Altitude = Model.Altitude
            };
            return Task.FromResult(newModel);
        }

        protected override async Task InitializeFromModelAsync(Alarm model)
        {
            AlarmName = model.Name;
            MapScreen = await _screenshotManager.OpenScreenFromPathAsync(model.MapScreenPath)
                .ConfigureAwait(true);
            SelectedNotificationSound = SystemSounds.TrimPrefix(model.AlarmSound);
            SelectedDays = ParseActiveDays(model.ActiveDays);
        }

        private void MediaPlayerOnMediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            var ars = args.ErrorMessage;
        }

        private List<string> ParseActiveDays(string activeDays) => string.IsNullOrEmpty(activeDays) ? new List<string>() : activeDays.Split(',').ToList();

        private string ParseSelectedDays() => SelectedDays.Aggregate("", (accu, name) => accu += $"{name},").TrimEnd(',');
    }
}