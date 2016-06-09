using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
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
    public class AlarmSettingsViewModel : ViewModelBase, INavigable
    {
        /// <summary>
        /// </summary>
        private readonly Dictionary<string, AlarmType> _alarmTypeMap = new Dictionary<string, AlarmType>();

        /// <summary>
        /// </summary>
        private readonly Dictionary<string, DayOfWeek> _dayOfWeekMap = new Dictionary<string, DayOfWeek>();

        /// <summary>
        /// </summary>
        private readonly MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;

        private readonly INavigationService _navigationService;

        /// <summary>
        /// </summary>
        private readonly ResourceLoader _resourceLoader;

        //BUG: HAck
        private AlarmModel _alarmModel = new AlarmModel();

        private Token _navigationToken;

        private string _selectedDaysConcated;
        public string AlarmName => _alarmModel.MonitoredArea.Name;

        /// <summary>
        /// </summary>
        public IEnumerable<string> AlarmTypes { get; private set; }

        /// <summary>
        /// </summary>
        public IList<string> DaysOfWeek { get; private set; }

        /// <summary>
        /// </summary>
        public BitmapImage MapScreen => _alarmModel.MapScreen;

        /// <summary>
        /// </summary>
        public IEnumerable<string> NotificationSounds { get; private set; } = new List<string> { "default" };

        /// <summary>
        /// </summary>
        public string SelectedAlarmType
        {
            get { return _resourceLoader.GetString(_alarmModel.AlarmType.ToString()); }
            set { _alarmModel.AlarmType = _alarmTypeMap[value]; }
        }

        /// <summary>
        /// </summary>
        public IEnumerable<string> SelectedDays
        {
            get { return _alarmModel.ActiveDays.Select(week => _resourceLoader.GetString(week.ToString())); }
            set
            {
                _alarmModel.ActiveDays.Clear();
                value.ForEach(day => _alarmModel.ActiveDays.Add(_dayOfWeekMap[day]));
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

        /// <summary>
        /// </summary>
        public string SelectedNotificationSound
        {
            get { return _alarmModel.NotificationSound; }
            set { _alarmModel.NotificationSound = value; }
        }

        /// <summary>
        /// </summary>
        public AlarmSettingsViewModel()
        {
            _resourceLoader = ResourceLoader.GetForCurrentView("Resources");

            SelectedDaysConcated = _resourceLoader.GetString("RepeatOnce");

            InitializeAlarmTypes();
            InitializeDaysOfWeek();
            InitializeAlaram();

            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(NavigationMessage message)
        {
            _alarmModel = null;
        }

        public async void OnNavigatedTo(NavigationMessage message)
        {
            _navigationToken = message.Token;

            _alarmModel = message.Data as AlarmModel;

            if (NotificationSounds.Count() <= 1)
                await InitializeSoundFileNames().ConfigureAwait(true);
            if (_navigationToken == Token.AddNew)
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
            _navigationService.NavigateTo(nameof(MainPage), new NavigationMessage(_navigationService.CurrentPageKey, _alarmModel, _navigationToken));
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

        /// <summary>
        /// </summary>
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

        private async Task InitializeSoundFileNames()
        {
            var notificationSounds = await ServiceLocator.Current
                .GetInstance<IAssetsNamesReader>()
                .ReadAsync("Sounds").ConfigureAwait(true);

            NotificationSounds = notificationSounds
                .Select(s => s.Replace(".mp3", ""));
        }
    }
}