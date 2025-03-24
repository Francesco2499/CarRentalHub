using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace Frontend.Converters;


public class VisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Verifica se il valore è una stringa
        if (value != null && value is string strValue && !string.IsNullOrEmpty(strValue))
        {
            // Se la stringa non è vuota o null, il controllo è visibile
            return true;
        }
        
        return false;  // Se il valore non è una stringa, non è visibile
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
