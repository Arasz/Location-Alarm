using CoreLibrary.Data.DataModel.PersistentModel;
using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LocationAlarm.Controls.AlarmItem
{
    public sealed partial class AlarmItemControl
    {
        private readonly double ManipulationDeltaXThreshold;
        private readonly double ManipulationStartThreshold;
        private double _deltaXSum;
        private bool _manipulationStarted;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<AlarmItemEventArgs> SwitchToggled;

        public event EventHandler<AlarmItemEventArgs> SwypeToDeleteCompleted;

        public AlarmItemControl()
        {
            InitializeComponent();
            ManipulationDeltaXThreshold = Width - 80;
            ManipulationStartThreshold = Width * 0.15;
        }

        private void AlarmItemControl_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
            if (!_manipulationStarted) return;

            if (_deltaXSum >= ManipulationDeltaXThreshold)
            {
                OnSwypeToDeleteCompleted();
                _manipulationStarted = false;
                return;
            }

            _deltaXSum = 0;
            RenderTransform = new TranslateTransform()
            {
                X = 0,
            };

            _manipulationStarted = false;
        }

        private void AlarmItemControl_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            if (!_manipulationStarted)
                return;

            _deltaXSum += e.Delta.Translation.X;
            RenderTransform = new TranslateTransform()
            {
                X = _deltaXSum,
            };
        }

        private void AlarmItemControl_OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Handled = true;
            if (_manipulationStarted && e.Position.X > ManipulationStartThreshold) return;

            _manipulationStarted = true;
        }

        private void OnSwitchToggled(Alarm model) => SwitchToggled?.Invoke(this, new AlarmItemEventArgs(model));

        private void OnSwypeToDeleteCompleted() => SwypeToDeleteCompleted?.Invoke(this, new AlarmItemEventArgs(DataContext as Alarm));

        private void ToggleSwitch_OnToggled(object sender, RoutedEventArgs e) => OnSwitchToggled(DataContext as Alarm);
    }
}