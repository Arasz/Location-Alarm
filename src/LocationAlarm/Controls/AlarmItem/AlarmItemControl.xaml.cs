using LocationAlarm.Model;
using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LocationAlarm.Controls.AlarmItem
{
    public sealed partial class AlarmItemControl : UserControl
    {
        private Point _endPosition;

        private double _manipulationDeltaXThreshold;

        private bool _manipulationStarted;
        private Point _startPosition;

        public event EventHandler<AlarmItemEventArgs> SwypeToDeleteCompleted;

        public AlarmItemControl()
        {
            InitializeComponent();
            _manipulationDeltaXThreshold = Width - 80;
            Debug.WriteLine(nameof(_manipulationDeltaXThreshold) + $": {_manipulationDeltaXThreshold}");
        }

        private void AlarmItemControl_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!_manipulationStarted) return;

            _manipulationStarted = false;
            _endPosition = e.Position;

            if ((_endPosition.X - _startPosition.X) >= _manipulationDeltaXThreshold)
                OnSwypeToDeleteCompleted();
        }

        private void AlarmItemControl_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (_manipulationStarted) return;

            _manipulationStarted = true;
            _startPosition = e.Position;
        }

        private void OnSwypeToDeleteCompleted()
        {
            SwypeToDeleteCompleted?.Invoke(this, new AlarmItemEventArgs(DataContext as AlarmModel));
        }
    }
}