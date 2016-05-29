using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary>
    public sealed partial class MainPage : BindablePage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// The methods provided in this section are simply used to allow NavigationHelper to respond
        /// to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/> and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method in addition to page state
        /// preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">
        /// Provides data for navigation methods and event handlers that cannot cancel the navigation request.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
}