using LocationAlarm.Navigation.Bundle;
using System;
using System.Threading.Tasks;

namespace LocationAlarm.Controllers
{
    public abstract class BaseController : IDisposable
    {
        public virtual async void Dispose()
        {
            await OnDestroyedAsync().ConfigureAwait(true);
        }

        public abstract Task InitializeViewAsync();

        public abstract Task InitializeViewModelAsync();

        public abstract Task OnDestroyedAsync();

        public abstract Task OnNavigatedFromAsync(Bundle bundle);

        public abstract Task OnNavigatedToAsync(Bundle bundle);
    }
}