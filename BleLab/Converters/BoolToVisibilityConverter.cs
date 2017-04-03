using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BleLab.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;

            if ((bool)value == bool.Parse(parameter as string))
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
