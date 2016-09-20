using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Shapes;

namespace LocationAlarm.Views.Utils
{
    public class MapCircleDrawer
    {
        private const double _earthRadiusInMeters = 6367000;
        private MapControl Map { get; }

        public MapCircleDrawer(MapControl map)
        {
            Map = map;
        }

        public void Draw(BasicGeoposition? geoposition, double circleRadius)
        {
            if (geoposition == null) return;

            var mapElements = Map.Children;

            mapElements.Where(element => element is Ellipse).Cast<Ellipse>().ForEach(ellipse =>
            {
                double metersPerPixel = (Math.Cos(ToRadian(geoposition.Value.Latitude)) * (2 * Math.PI * _earthRadiusInMeters)) / (256 * Math.Pow(2, Map.ZoomLevel));
                ellipse.Height = circleRadius / metersPerPixel * 2;
                ellipse.Width = circleRadius / metersPerPixel * 2;
            });
        }

        private static double ToRadian(double degrees) => degrees * (Math.PI / 180);
    }
}