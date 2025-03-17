using System.Globalization;
using System.Windows.Data;

namespace Musicx.Ui.Converters;

public class SliderTrackFillConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length == 3 && values[0] is double value && values[1] is double max && values[2] is double width)
        {
            if (max == 0) return 0;
            return value / max * width; // Calcul de la largeur de la Track remplie
        }
        return 0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}