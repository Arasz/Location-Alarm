using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    internal class AutoSuggestBoxSuggestionChosenEventArgsToSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as AutoSuggestBoxSuggestionChosenEventArgs;
            return args?.SelectedItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}