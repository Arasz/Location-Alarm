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

        private readonly Dictionary<string, DayOfWeek> _dayOfWeekMap = new Dictionary<string, DayOfWeek>();

        private readonly MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;

        private readonly ResourceLoader _resourceLoader;

        private string _selectedDaysConcated;
        public string AlarmName => _selectedAlarm.MonitoredArea.Name;

        public IEnumerable<string> AlarmTypes { get; private set; }

        public IList<string> DaysOfWeek { get; private set; }

        public BitmapImage MapScreen => _selectedAlarm.MapScreen;

        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string> { "default" };

        public string SelectedAlarmType
        {
            get { return _resourceLoader.GetString(_selectedAlarm.AlarmType.ToString()); }
            set { _selectedAlarm.AlarmType = _alarmTypeMap[value]; }
        }

        public IEnumerable<string> SelectedDays
        {
            get { return _selectedAlarm.ActiveDays.Select(week => _resourceLoader.GetString(week.ToString())); }
            set
            {
                _selectedAlarm.ActiveDays.Clear();
                value.ForEach(day => _selectedAlarm.ActiveDays.Add(_dayOfWeekMap[day]));
            }
        }

        public string SelectedDaysConcated
        {
            get
            {
                var stringRepresentation = SelectedDays.Aggregate("", (agg, day) => agg + $", {day.TrimStart(' ').Substring(0, 3)}").TrimStart(',', ' ');
                _selectedDaysConcated = string.IsNullOrEmpty(stringRepresentation) ? _resourceLoader.GetString("RepeatOnce") : stringRepresentation;
                return _selectedDaysConcated;
            }

            set { _selectedDaysConcated = value; }
        }

        public string SelectedNotificationSound
        {
            get { return _selectedAlarm.NotificationSound; }
            set { _selectedAlarm.NotificationSound = value; }
        }

        public AlarmSettingsViewModel(NavigationServiceWithToken navigationService) : base(navigationService)
        {
            _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

            SelectedDaysConcated = _resourceLoader.GetString("RepeatOnce");

            InitializeAlarmTypes();
            InitializeDaysOfWeek();
            InitializeAlaram();
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

        [OnCommand("RepeatWeeklyClosedCommand")]
        public void OnRepeatWeeklyClosed() => RaisePropertyChanged(nameof(SelectedDaysConcated));

        [OnCommand("SaveSettingsCommand")]
        public void OnSaveAlarmSettings()
        {
            _navigationService.NavigateTo(nameof(MainPage));
        }

        private void InitializeAlaram()
        {
            SelectedAlarmType = AlarmTypes.First();
            SelectedNotificationSound = NotificationSounds.First();
        }

        private void InitializeAlarmTypes()
        {
            AlarmTypes = Enum.GetNames(typeof(AlarmType))
                .Select(enumName =>
                {
                    var alarmTypeName = _resourceLoader.GetString(enumName);
                    _alarmTypeMap[alarmTypeName] = (AlarmType)Enum.Parse(typeof(AlarmType), enumName);
                    return alarmTypeName;
                }).ToList();
        }

        private void InitializeDaysOfWeek()
        {
            DaysOfWeek = Enum.GetNames(typeof(DayOfWeek))
                .Select(enumName =>
                {
                    var dayOfWeekName = _resourceLoader.GetString(enumName);
                    _dayOfWeekMap[dayOfWeekName] = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), enumName);
                    return dayOfWeekName;
                })
                .ToList();
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