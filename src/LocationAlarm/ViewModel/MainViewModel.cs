using ArrivalAlarm.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using LocationAlarm.Model;
using LocationAlarm.Navigation;
using LocationAlarm.View;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Windows.Devices.Geolocation;

namespace LocationAlarm.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to. 
    /// <para>
    /// Use the <strong> mvvminpc </strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para> You can also use Blend to data bind with the tool's support. </para>
    /// <para> See http://www.galasoft.ch/mvvm </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INavigable
    {
        private readonly ObservableCollection<AlarmModel> _alarmsCollection = new ObservableCollection<AlarmModel>()
        {
            new AlarmModel(new MonitoredArea("Poznan", new GeofenceBuilder().SetRequiredId("P1").ThenSetGeocircle(new BasicGeoposition(),4d)))
            {
                Label = "Alarm praca",
                ActiveDays = new HashSet<DayOfWeek>() {DayOfWeek.Monday, DayOfWeek.Tuesday},
                IsActive = true,
                IsCyclic = true,
            },

            new AlarmModel(new MonitoredArea("Wroc³aw", new GeofenceBuilder().SetRequiredId("W1").ThenSetGeocircle(new BasicGeoposition(),6d)))
            {
                Label = "Uczelnia",
                ActiveDays = new HashSet<DayOfWeek>() {DayOfWeek.Friday, DayOfWeek.Wednesday},
                IsActive = true,
                IsCyclic = true,
            },
        };

        private RelayCommand _navigateToSelectLocationPage;
        private INavigationService _navigationService;
        public INotifyCollectionChanged AlarmsCollection => _alarmsCollection;

        /// <summary>
        /// Returns command which navigates to selection page 
        /// </summary>
        public ICommand NavigateToSelectLocationPage => _navigateToSelectLocationPage;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class. 
        /// </summary>
        public MainViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

            CreateNavigateToSelectLocationPageCommand();
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
            //Common.Logger.CreateLoggerAsync();
        }

        private void CreateNavigateToSelectLocationPageCommand()
        {
            _navigateToSelectLocationPage = new RelayCommand(ExecuteNavigateToLocationPage);
        }

        private void ExecuteNavigateToLocationPage()
        {
            _navigationService.NavigateTo(nameof(MapPage), "Graf acykliczny - mo¿e byæ reprezentowany jako drzewo.");
        }
    }
}