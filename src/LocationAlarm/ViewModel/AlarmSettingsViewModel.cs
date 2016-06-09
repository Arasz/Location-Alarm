using Commander;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.Utils;
using LocationAlarm.View;
using Microsoft.Practices.ServiceLocation;
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

        //BUG: HAck
        private AlarmModel _alarmModel = new AlarmModel();

        private Dictionary<string, AlarmType> _alarmTypeMap = new Dictionary<string, AlarmType>();
        private Dictionary<string, DayOfWeek> _dayOfWeekMap = new Dictionary<string, DayOfWeek>();

        private MediaPlayer _mediaPlayer = BackgroundMediaPlayer.Current;

        private IEnumerable<string> _selectedDays;

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
        public IEnumerable<string> NotificationSounds { get; private set; }

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
            get { return _selectedDays; }
            set
            {
                _selectedDays = value;
                _alarmModel.ActiveDays.Clear();
                value.ForEach(day => _alarmModel.ActiveDays.Add(_dayOfWeekMap[day]));
            }
        }

        public string SelectedDaysConcated { get; set; }

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
            InitializeSoundFileNames();

            _navigationService = SimpleIoc.Default.GetInstance<INavigationService>();
        }

        public void GoBack()
        {
            _navigationService.GoBack();
        }

        public void OnNavigatedFrom(NavigationMessage message)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationMessage message)
        {
            _alarmModel = message.Data as AlarmModel;
        }

        [OnCommand("PlaySoundCommand")]
        public void OnPlaySound(string soundName)
        {
            _mediaPlayer.SetUriSource(new Uri(_resourceLoader.GetString("SoundFileAppendix") + soundName + ".mp3"));
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
            _navigationService.NavigateTo(nameof(MainPage), _alarmModel);
        }

        private void InitializeAlarmTypes()
        {
            AlarmTypes = Enum.GetNames(typeof(AlarmType))
                .Select(enumName =>
                {
                    var alarmTypeName = _resourceLoader.GetString(enumName);
                    _alarmTypeMap[alarmTypeName] = (AlarmType)Enum.Parse(typeof(AlarmType), enumName);
                    return alarmTypeName;
                })
                .ToList();
            SelectedAlarmType = AlarmTypes.First();
        }

        /// <summary>
        /// </summary>
        private void InitializeDaysOfWeek()
        {
            DaysOfWeek = Enum.GetNames(typeof(DayOfWeek))
                .Select(enumName =>
                {
                    var dayOfWeekName = "  " + _resourceLoader.GetString(enumName);
                    _dayOfWeekMap[dayOfWeekName] = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), enumName);
                    return dayOfWeekName;
                })
                .ToList();
        }

        private async void InitializeSoundFileNames()
        {
            var notificationSounds = await ServiceLocator.Current.GetInstance<IAssetsNamesReader>().ReadAsync("Sounds").ConfigureAwait(true);
            NotificationSounds = notificationSounds.Select(s => s.Replace(".mp3", ""));
            SelectedNotificationSound = NotificationSounds.First();
        }
    }
}