using System.Globalization;
using System.Windows.Data;

namespace Musicx.Ui.Converters;

public class FloatToPercentageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double volume)
        {
            return $"{(int)(volume * 100)}%";
        }
        
        if (value is float floatVolume)
        {
            return $"{(int)(floatVolume * 100)}%";
        }
        
        return "0%";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str && str.EndsWith("%") && int.TryParse(str.TrimEnd('%'), out int percentage))
        {
            return percentage / 100.0;
        }
        return 0.0;
    }
}