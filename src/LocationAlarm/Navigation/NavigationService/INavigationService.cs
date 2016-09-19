using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationAlarm.Navigation.NavigationService
{
    public interface INavigationService
    {
        Type Current { get; }

        Stack<Type> NavigationHistory { get; }

        Task GoBackAsync();

        Task NavigateToAsync(Type controllerType, Bundle.Bundle bundle);
    }
}