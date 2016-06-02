using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Media.Playback;
using Windows.UI.Xaml.Media.Imaging;

namespace LocationAlarm.ViewModel
{
    [ImplementPropertyChanged]
    public class AlarmSettingsViewModel : ViewModelBase, INavigable
    {
        private readonly INavigationService _navigationService;

        /// <summary>
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        private AlarmModel _alarmModel;

        private string _appendix = "ms-appx:///Assets/Sounds/";
        private MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;

        /// <summary>
        /// </summary>
        public IEnumerable<string> AlarmTypes { get; private set; }

        /// <summary>
        /// </summary>
        public IList<string> DaysOfWeek { get; private set; }

        /// <summary>
        /// </summary>
        public BitmapImage MapScreen { get; private set; }

        /// <summary>
        /// </summary>
        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string>
        { "Unique Notification0.mp3"};

        /// <summary>
        /// </summary>
        public string SelectedAlarmType { get; set; }

        /// <summary>
        /// </summary>
        public IEnumerable<string> SelectedDays { get; set; }

        public string SelectedDaysConcated { get; set; }

        /// <summary>
        /// </summary>
        public string SelectedNotificationSound { get; set; }

        /// <summary>
        /// </summary>
        public AlarmSettingsViewModel()
        {
            _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

            SelectedDaysConcated = _resourceLoader.GetString("RepeatOnce");
            SelectedNotificationSound = NotificationSounds.First();

            InitializeAlarmTypes();
            InitializeDaysOfWeek();

            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(object parameter)
        {
        }

        public void OnNavigatedTo(object parameter)
        {
            MapScreen = parameter as BitmapImage;
        }

        [OnCommand("PlaySoundCommand")]
        public void OnPlaySound()
        {
            _mediaPlayer.SetUriSource(new Uri(_appendix + SelectedNotificationSound));
            _mediaPlayer.Play();
        }

        [OnCommand("RepeatWeeklyClosedCommand")]
        public void OnRepeatWeeklyClosed()
        {
            var stringRepresentation = SelectedDays.Aggregate("", (agg, day) => agg + $", {day.TrimStart(' ').Substring(0, 3)}").TrimStart(',', ' ');

            SelectedDaysConcated = string.IsNullOrEmpty(stringRepresentation) ? _resourceLoader.GetString("RepeatOnce") : stringRepresentation;
        }

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings()
        {
        }

        private void InitializeAlarmTypes()
        {
            AlarmTypes = _resourceLoader.GetString("AlarmSettingsAlarmTypeCollection").Split(',').Select(s => s.Trim());
            SelectedAlarmType = AlarmTypes.First();
        }

        /// <summary>
        /// </summary>
        private void InitializeDaysOfWeek()
        {
            DaysOfWeek = Enum.GetNames(typeof(DayOfWeek)).Select(s => "  " + _resourceLoader.GetString(s)).ToList();
        }
    }
}