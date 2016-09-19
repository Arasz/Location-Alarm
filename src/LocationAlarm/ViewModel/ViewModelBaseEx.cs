﻿using CoreLibrary.StateManagement;
using GalaSoft.MvvmLight;
using LocationAlarm.Navigation;
using System.Threading.Tasks;

namespace LocationAlarm.ViewModel
{
    public abstract class ViewModelBaseEx<TModel> : ViewModelBase, INavigable
    {
        protected readonly NavigationServiceWithToken _navigationService;

        protected StateManager<TModel> AlarmStateManager { get; set; }

        protected TModel Model { get; set; }

        protected ViewModelBaseEx(NavigationServiceWithToken navigationService)
        {
            _navigationService = navigationService;
        }

        public virtual void GoBack()
        {
            _navigationService.GoBack();
        }

        public virtual void OnNavigatedFrom(object parameter)
        {
        }

        public virtual void OnNavigatedTo(object parameter)
        {
        }

        protected abstract Task<TModel> CreateModelAsync();

        protected abstract Task InitializeFromModelAsync(TModel model);
    }
}