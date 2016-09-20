using CoreLibrary.Utils.ScreenshotManager;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using LocationAlarm.Common;
using LocationAlarm.ViewModels;
using LocationAlarm.Views.Utils;
using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.Views
{
    public sealed partial class MapPage
    {
        private readonly MapCircleDrawer _mapCircleDrawer;

        private readonly ScreenshotManager _screenshotManager;

        private readonly MapViewModel _viewModel;

        public event EventHandler MapScreenshootTaken;

        public MapPage()
        {
            InitializeComponent();

            _viewModel = DataContext as MapViewModel;
            _mapCircleDrawer = new MapCircleDrawer(mapControl);
            _screenshotManager = new ScreenshotManager(ApplicationData.Current.LocalFolder);

            mapControl.Tapped += MapControlOnTapped;
            mapControl.PitchChanged += MapControlOnPitchChanged;
            mapControl.ZoomLevelChanged += MapControlOnZoomLevelChanged;

            Messenger.Default.Register<Geopoint>(this, ViewModelOnCurrentLocationLoaded);
            Messenger.Default.Register<MessageBase>(this, Token.TakeScreenshot, TakeMapScreenshotAsync);
            Messenger.Default.Register<MessageBase>(this, Token.FocusOnMap, SetFocusOnMap);
        }

        private void MapControlOnPitchChanged(MapControl sender, object args)
        {
            mapControl.Children
                .Where(o => o is Ellipse)
                .Cast<Ellipse>()
                .ForEach(ellipse =>
            {
                ellipse.Projection = new PlaneProjection()
                {
                    CenterOfRotationX = 0,
                    CenterOfRotationY = .5,
                    CenterOfRotationZ = 0,
                    RotationX = mapControl.Pitch,
                };
            });
        }

        private void MapControlOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs) => mapControl.Focus(FocusState.Pointer);

        private void MapControlOnZoomLevelChanged(MapControl sender, object args)
        {
            _viewModel.ZoomLevel = sender.ZoomLevel;
            _mapCircleDrawer.Draw(_viewModel?.ActualLocation, _viewModel.GeocircleRadius);
        }

        private void RangeBase_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e) => _mapCircleDrawer.Draw(_viewModel?.ActualLocation, _viewModel.GeocircleRadius);

        private void SetFocusOnMap(MessageBase message) => mapControl.Focus(FocusState.Pointer);

        private void TakeMapScreenshotAsync(MessageBase message)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(async () =>
            {
                if (mapControl.RenderSize == new Size(0, 0))
                    return;

                var path = await _screenshotManager.TakeScreenshotAsync(mapControl, DisplayInformation.GetForCurrentView(), _viewModel.AutoSuggestionLocationQuery)
                    .ConfigureAwait(true);

                _viewModel.MapScreenshotPath = path;
            });
        }

        private void ViewModelOnCurrentLocationLoaded(Geopoint geopoint)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(
                async () =>
                {
                    LoadingProgressBar.Opacity = 100;
                    await mapControl.TrySetViewAsync(geopoint, _viewModel.ZoomLevel, 0, 0, MapAnimationKind.Linear);
                    LoadingProgressBar.Opacity = 0;
                }
                );
        }
    }
}