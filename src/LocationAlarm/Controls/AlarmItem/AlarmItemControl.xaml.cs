using CoreLibrary.Data.DataModel.PersistentModel;
using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LocationAlarm.Controls.AlarmItem
{
    public sealed partial class AlarmItemControl
    {
        private double _deltaXSum = 0;
        private Point _endPosition;
        private double _manipulationDeltaXThreshold;
        private bool _manipulationStarted;
        private Point _startPosition;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<AlarmItemEventArgs> SwitchToggled;

        public event EventHandler<AlarmItemEventArgs> SwypeToDeleteCompleted;

        public AlarmItemControl()
        {
            InitializeComponent();
            _manipulationDeltaXThreshold = Width - 80;
        }

        private void AlarmItemControl_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (!_manipulationStarted) return;

            _manipulationStarted = false;
            _endPosition = e.Position;

            if (_deltaXSum >= _manipulationDeltaXThreshold)
                OnSwypeToDeleteCompleted();

            _deltaXSum = 0;
            RenderTransform = new TranslateTransform()
            {
                X = 0,
            };
        }

        private void AlarmItemControl_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (!_manipulationStarted) return;
            _deltaXSum += e.Delta.Translation.X;
            RenderTransform = new TranslateTransform()
            {
                X = _deltaXSum,
            };
        }

        private void AlarmItemControl_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            if (_manipulationStarted) return;

            _manipulationStarted = true;
            _startPosition = e.Position;
        }

        private void OnSwitchToggled(Alarm model) => SwitchToggled?.Invoke(this, new AlarmItemEventArgs(model));

        private void OnSwypeToDeleteCompleted() => SwypeToDeleteCompleted?.Invoke(this, new AlarmItemEventArgs(DataContext as Alarm));

        private void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e) => OnSwitchToggled(DataContext as Alarm);
    }
}