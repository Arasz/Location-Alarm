using LocationAlarm.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace LocationAlarm.View
{
    /// <summary>
    /// Page implementation with navigation event handlers consistent with MVVM pattern 
    /// </summary>
    public class BindablePage : Page
    {
        /// <summary>
        /// Invoked when this page is about to be removed from Frame 
        /// </summary>
        /// <param name="e">
        /// Event data that describes how this page was reached. This parameter is typically used to
        /// configure the page.
        /// </param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;

            var navigableViewModel = DataContext as INavigable;

            navigableViewModel?.OnNavigatedFrom(e.Parameter as NavigationMessage);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame. 
        /// </summary>
        /// <param name="e">
        /// Event data that describes how this page was reached. This parameter is typically used to
        /// configure the page.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            var navigableViewModel = DataContext as INavigable;

            navigableViewModel?.OnNavigatedTo(e.Parameter as NavigationMessage);
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            var navigableViewModel = DataContext as INavigable;

            e.Handled = true;
            navigableViewModel?.GoBack();
        }
    }
}