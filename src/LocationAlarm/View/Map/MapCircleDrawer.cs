using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Shapes;

namespace LocationAlarm.View.Map
{
    public class MapCircleDrawer
    {
        public MapControl Map { get; }

        public MapCircleDrawer(MapControl map)
        {
            Map = map;
        }

        public static double ToDegrees(double radians) => radians * (180 / Math.PI);

        public static double ToRadian(double degrees) => degrees * (Math.PI / 180);

        public void Draw(BasicGeoposition position, double circleRadius)
        {
            var mapElements = Map.Children;
            double earthRadiusInMeters = 6367.0 * 1000.0;

            mapElements.Where(element => element is Ellipse).Cast<Ellipse>().ForEach(ellipse =>
            {
                double metersPerPixel = (Math.Cos(ToRadian(position.Latitude)) * (2 * Math.PI * earthRadiusInMeters)) / (256 * Math.Pow(2, Map.ZoomLevel));
                ellipse.Height = circleRadius / metersPerPixel * 2;
                ellipse.Width = circleRadius / metersPerPixel * 2;
            });
        }
    }
}