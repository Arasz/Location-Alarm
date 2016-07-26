using Autofac;

namespace LocationAlarm.ViewModel
{
    public class ViewModelLocator
    {
        private readonly ILifetimeScope _scope;

        public AlarmSettingsViewModel AlarmSettings => _scope.Resolve<AlarmSettingsViewModel>();

        public MainViewModel Main => _scope.Resolve<MainViewModel>();

        public MapViewModel Map => _scope.Resolve<MapViewModel>();

        public ViewModelLocator(ILifetimeScope scope)
        {
            _scope = scope;
        }
    }
}