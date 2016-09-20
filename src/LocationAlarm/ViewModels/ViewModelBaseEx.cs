using GalaSoft.MvvmLight;
using LocationAlarm.Navigation;
using System.Threading.Tasks;

namespace LocationAlarm.ViewModels
{
    public abstract class ViewModelBaseEx<TModel> : ViewModelBase, INavigable
        where TModel : new()
    {
        protected readonly NavigationServiceWithToken _navigationService;

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

        protected virtual void InitializeViewModel(TModel dataSource)
        {
        }

        protected virtual Task InitializeViewModelAsync(TModel dataSource) => Task.Run(() => InitializeViewModel(dataSource));

        protected virtual TModel SaveDataToModel(TModel prototype) => new TModel();

        protected virtual Task SaveDataToModelAsync(TModel prototype) => Task.Run(() => SaveDataToModel(prototype));
    }
}