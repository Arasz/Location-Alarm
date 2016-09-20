// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

using LocationAlarm.ViewModel;
using System.Globalization;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace LocationAlarm.View
{
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
            var flyout = RepeatSettingsButton.Flyout as ListPickerFlyout;
            var sortedDays = DateTimeFormatInfo.CurrentInfo.DayNames;
            var selectedDays = flyout.SelectedItems.Cast<string>().ToList();
            _viewModel.SelectedDays = sortedDays.Where(day => selectedDays.Contains(day)).ToList();
        }

        private void FlyoutBase_OnOpening(object sender, object e)
        {
            var flyout = RepeatSettingsButton.Flyout as ListPickerFlyout;
            flyout.SelectedItems.Clear();
            var source = _viewModel.DaysOfWeek;

            _viewModel.SelectedDays.ForEach(weekDay => flyout.SelectedItems.Add(source.First(day => day == weekDay)));
        }
    }
}