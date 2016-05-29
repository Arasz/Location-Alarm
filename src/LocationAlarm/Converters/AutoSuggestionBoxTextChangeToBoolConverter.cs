using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LocationAlarm.Converters
{
    internal class AutoSuggestionBoxTextChangeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var eventArgs = value as AutoSuggestBoxTextChangedEventArgs;

            return eventArgs?.Reason == AutoSuggestionBoxTextChangeReason.UserInput;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}