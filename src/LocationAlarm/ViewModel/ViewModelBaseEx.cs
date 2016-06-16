﻿using GalaSoft.MvvmLight;
using LocationAlarm.Model;
using LocationAlarm.Navigation;

namespace LocationAlarm.ViewModel
{
    public class ViewModelBaseEx : ViewModelBase, INavigable
    {
        //TODO: Shouldn't be static
        protected static volatile Model.LocationAlarm SelectedLocationAlarm;

        protected readonly NavigationServiceWithToken _navigationService;

        public ViewModelBaseEx(NavigationServiceWithToken navigationService)
        {
            _navigationService = navigationService;
        }

        public virtual void GoBack()
        {
            _navigationService.GoBack();
        }

        public virtual void OnNavigatedFrom(NavigationMessage message)
        {
        }

        public virtual void OnNavigatedTo(NavigationMessage message)
        {
        }
    }
}