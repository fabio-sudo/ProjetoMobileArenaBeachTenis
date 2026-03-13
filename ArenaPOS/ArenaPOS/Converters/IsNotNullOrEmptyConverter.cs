using System.Globalization;
using Microsoft.Maui.Controls;

namespace ArenaPOS.Converters;

public class IsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value?.ToString());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}