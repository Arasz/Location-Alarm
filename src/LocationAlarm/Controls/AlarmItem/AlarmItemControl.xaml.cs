using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LocationAlarm.Controls.AlarmItem
{
    public sealed partial class AlarmItemControl : UserControl
    {
        private Point _endPosition;

        private double _manipulationDeltaXThreshold = 340d;

        private bool _manipulationStarted;
        private Point _startPosition;

        public event EventHandler SwypeToDeleteCompleted;

        public AlarmItemControl()
        {
            InitializeComponent();
        }

        private void AlarmItemControl_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!_manipulationStarted) return;

            _manipulationStarted = false;
            _endPosition = e.Position;
        }

        private void AlarmItemControl_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!_manipulationStarted) return;
            var delta = e.Delta;
            if (delta.Translation.X >= _manipulationDeltaXThreshold)
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
            SwypeToDeleteCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}