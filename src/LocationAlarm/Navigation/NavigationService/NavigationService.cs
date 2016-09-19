using Autofac;
using LocationAlarm.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.Navigation.NavigationService
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _navigationFrame;
        private readonly ILifetimeScope _scope;
        private BaseController _currentController;

        public Type Current { get; private set; }

        public Stack<Type> NavigationHistory { get; }

        public NavigationService(ILifetimeScope scope, Frame navigationFrame)
        {
            _scope = scope;
            _navigationFrame = navigationFrame;
            NavigationHistory = new Stack<Type>();
        }

        public async Task GoBackAsync()
        {
            if (NavigationHistory.Count == 1)
                return;

            var bundle = new Bundle.Bundle();
            await _currentController.OnNavigatedFromAsync(bundle).ConfigureAwait(true);

            NavigationHistory.Pop();
            Current = NavigationHistory.Peek();

            await ControllerInitializationAsync().ConfigureAwait(true);
            await _currentController.OnNavigatedToAsync(bundle).ConfigureAwait(true);
        }

        public void Initialize(Type controllerType)
        {
            NavigationHistory.Push(controllerType);
            Current = controllerType;
        }

        public async Task NavigateToAsync(Type controllerType, Bundle.Bundle bundle)
        {
            await _currentController.OnNavigatedFromAsync(bundle).ConfigureAwait(true);

            Initialize(controllerType);
            await ControllerInitializationAsync().ConfigureAwait(true);

            await _currentController.OnNavigatedToAsync(bundle).ConfigureAwait(true);
        }

        private async Task ControllerInitializationAsync()
        {
            _currentController = _scope.Resolve(Current) as BaseController;
            await _currentController.InitializeViewModelAsync().ConfigureAwait(true);
            await _currentController.InitializeViewAsync().ConfigureAwait(true);
        }
    }
}