// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

using LocationAlarm.ViewModels;

namespace LocationAlarm.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary>
    public sealed partial class MainPage
    {
        private MainViewModel _viewModel;

        public MainPage()
        {
            InitializeComponent();
            _viewModel = DataContext as MainViewModel;
        }
    }
}