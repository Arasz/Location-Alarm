using GalaSoft.MvvmLight.Views;
using LocationAlarm.Common;

namespace LocationAlarm.Navigation
{
    public interface INavigationServiceWithToken : INavigationService
    {
        string LastPageKey { get; set; }

        Token Token { get; set; }
    }
}