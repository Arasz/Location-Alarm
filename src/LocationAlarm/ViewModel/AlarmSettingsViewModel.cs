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

        private volatile Alarm _model;

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
        public void EditLocation() => _navigationService.NavigateTo(nameof(MapPage), SaveDataToModel(_model));

        public override void GoBack() => _navigationService.GoBack();

        public override async void OnNavigatedTo(object parameter)
        {
            _model = parameter as Alarm;

            await InitializeViewModelAsync(_model).ConfigureAwait(false);
        }

        [OnCommand("PlaySoundCommand")]
        public void OnPlaySound(string soundName)
        {
            var uri = new Uri(SystemSounds.NameToUriMap[soundName], UriKind.RelativeOrAbsolute);
            _mediaPlayer.SetUriSource(uri);
            _mediaPlayer.Play();
        }

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings() => _navigationService.NavigateTo(nameof(MainPage), SaveDataToModel(_model));

        protected override async Task InitializeViewModelAsync(Alarm dataSource)
        {
            AlarmName = dataSource.Name;
            MapScreen = await _screenshotManager.OpenScreenFromPathAsync(dataSource.MapScreenPath)
                .ConfigureAwait(true);
            SelectedNotificationSound = SystemSounds.TrimPrefix(dataSource.AlarmSound);
            SelectedDays = ParseActiveDays(dataSource.ActiveDays);
        }

        protected override Alarm SaveDataToModel(Alarm prototype)
        {
            var newModel = new Alarm
            {
                ActiveDays = ParseSelectedDays(),
                AlarmSound = SystemSounds.NameToUriMap[SelectedNotificationSound],
                AlarmType = SelectedAlarmType,

                Id = prototype.Id,
                Name = prototype.Name,
                IsActive = prototype.IsActive,
                MapScreenPath = prototype.MapScreenPath,
                Radius = prototype.Radius,
                Latitude = prototype.Latitude,
                Longitude = prototype.Longitude,
                Altitude = prototype.Altitude
            };
            return newModel;
        }

        private List<string> ParseActiveDays(string activeDays) => string.IsNullOrEmpty(activeDays) ? new List<string>() : activeDays.Split(',').ToList();

        private string ParseSelectedDays() => SelectedDays.Aggregate("", (accu, name) => accu += $"{name},").TrimEnd(',');
    }
}