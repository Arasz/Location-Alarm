using ArrivalAlarm.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.Devices.Geolocation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace ArrivalAlarm.View
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

            Messenger.Default.Register<Geopoint>(this, Messages.Tokens.MapViewToken, SetMapViewAsync);
        }

        private async void SetMapViewAsync(Geopoint location)
        {
            await mapControl.TrySetViewAsync(location, _viewModel.ZoomLevel, 0, 0, Windows.UI.Xaml.Controls.Maps.MapAnimationKind.Bow);
        }
    }
}