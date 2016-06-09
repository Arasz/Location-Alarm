using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LocationAlarm.Controls
{
    public sealed partial class AlarmItemControl : UserControl
    {
        public event RoutedEventHandler DeleteButtonClicked
        {
            add { DeleteButton.Click += value; }
            remove { DeleteButton.Click -= value; }
        }

        public AlarmItemControl()
        {
            InitializeComponent();
        }
    }
}