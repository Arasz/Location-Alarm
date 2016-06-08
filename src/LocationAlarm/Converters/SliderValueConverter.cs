using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    public class SliderValueConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register(nameof(Parameter), typeof(object), typeof(SliderValueConverter), new PropertyMetadata(null));

        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var minValue = (double)Parameter;
            var meters = (double)value;
            var retVal = minValue + Math.Log((meters - minValue) + 1);
            return retVal;
        }
    }
}