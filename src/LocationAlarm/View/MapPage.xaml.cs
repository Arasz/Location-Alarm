using ArrivalAlarm.Messages;
using GalaSoft.MvvmLight.Messaging;
using LocationAlarm.Common;
using LocationAlarm.View.Map;
using LocationAlarm.ViewModel;
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.View
{
    public sealed partial class MapPage
    {
        private readonly MapCircleDrawer _mapCircleDrawer;
        private readonly MapViewModel _viewModel;

        public MapPage()
        {
            InitializeComponent();

            _viewModel = DataContext as MapViewModel;
            _mapCircleDrawer = new MapCircleDrawer(mapControl);

            _viewModel.CurrentLocationLoaded += ViewModelOnCurrentLocationLoaded;
            Messenger.Default.Register<MapMessage>(this, Token.TakeScreenshot, TakeMapScreenshotAsync);
            Messenger.Default.Register<MapMessage>(this, Token.FocusOnMap, SetFocusOnMap);

            mapControl.Tapped += MapControlOnTapped;
            mapControl.PitchChanged += MapControlOnPitchChanged;
            mapControl.ZoomLevelChanged += MapControlOnZoomLevelChanged;
        }

        private void MapControlOnPitchChanged(MapControl sender, object args)
        {
            mapControl.Children
                .Where(o => o is Ellipse)
                .Cast<Ellipse>()
                .ForEach(ellipse =>
            {
                //ellipse.RenderTransformOrigin = new Point(0, .5);
                ellipse.Projection = new PlaneProjection()
                {
                    CenterOfRotationX = 0,
                    CenterOfRotationY = .5,
                    CenterOfRotationZ = 0,
                    RotationX = mapControl.Pitch,
                };
            });
        }

        private void MapControlOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            mapControl.Focus(FocusState.Pointer);
        }

        private void MapControlOnZoomLevelChanged(MapControl sender, object args)
        {
            _mapCircleDrawer.Draw(_viewModel?.ActualLocation, _viewModel.GeocircleRadius);
        }

        private void RangeBase_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _mapCircleDrawer.Draw(_viewModel?.ActualLocation, _viewModel.GeocircleRadius);
        }

        private void SetFocusOnMap(MapMessage mapMessage)
        {
            mapControl.Focus(FocusState.Pointer);
        }

        /// <summary>
        /// </summary>
        private async void TakeMapScreenshotAsync(MapMessage mapMessage)
        {
            if (mapControl.RenderSize == new Size(0, 0))
                return;
            var dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var renderTargetBitmap = new RenderTargetBitmap();
            await renderTargetBitmap.RenderAsync(mapControl);
            var pixelBuffer = await renderTargetBitmap.GetPixelsAsync();
            var randomAccessStream = new InMemoryRandomAccessStream();

            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore,
                (uint)renderTargetBitmap.PixelWidth,
                (uint)renderTargetBitmap.PixelHeight,
                dpi,
                dpi,
                pixelBuffer.ToArray());

            await encoder.FlushAsync();

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(randomAccessStream);

            _viewModel.MapScreenshot = bitmapImage;
        }

        private async Task ViewModelOnCurrentLocationLoaded(object o, Geopoint geopoint)
        {
            LoadingProgressBar.Opacity = 100;
            await mapControl.TrySetViewAsync(geopoint, _viewModel.ZoomLevel, 0, 0, MapAnimationKind.Linear);
            LoadingProgressBar.Opacity = 0;
        }
    }
}