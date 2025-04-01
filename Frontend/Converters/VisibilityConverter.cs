using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Frontend.Converters;


public class VisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null && value is string strValue && !string.IsNullOrEmpty(strValue);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
