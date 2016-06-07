using ArrivalAlarm.Messages;
using GalaSoft.MvvmLight.Messaging;
using LocationAlarm.ViewModel;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAlarm.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. 
    /// </summary>
    public sealed partial class MapPage
    {
        private readonly MapViewModel _viewModel;

        public MapPage()
        {
            InitializeComponent();

            _viewModel = DataContext as MapViewModel;

            mapControl.LoadingStatusChanged += (sender, args) =>
            {
                if (sender.LoadingStatus == MapLoadingStatus.Loaded)
                {
                    Messenger.Default.Send(true, Tokens.MapLoaded);
                }
            };
            Messenger.Default.Register<Geopoint>(this, Tokens.SetMapView, SetMapViewAsync);
            Messenger.Default.Register<MapMessage>(this, Tokens.TakeScreenshot, TakeMapScreenshotAsync);
            Messenger.Default.Register<MapMessage>(this, Tokens.FocusOnMap, SetFocusOnMap);
        }

        private void SetFocusOnMap(MapMessage mapMessage)
        {
            mapControl.Focus(FocusState.Pointer);
        }

        /// <summary>
        /// </summary>
        /// <param name="location"></param>
        private async void SetMapViewAsync(Geopoint location)
        {
            await mapControl.TrySetViewAsync(location, _viewModel.ZoomLevel, 0, 0, MapAnimationKind.Bow);
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
    }
}