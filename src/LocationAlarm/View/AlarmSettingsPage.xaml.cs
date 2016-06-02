// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

using LocationAlarm.ViewModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary>
    public sealed partial class AlarmSettingsPage : BindablePage
    {
        private AlarmSettingsViewModel _viewModel;

        public AlarmSettingsPage()
        {
            InitializeComponent();
            _viewModel = DataContext as AlarmSettingsViewModel;
        }

        private void FlyoutBase_OnClosed(object sender, object e)
        {
            _viewModel.SelectedDays = (RepeatSettingsButton.Flyout as ListPickerFlyout).SelectedItems.Cast<string>();
            _viewModel.OnRepeatWeeklyClosed();
        }
    }
}