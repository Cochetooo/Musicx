using System.Globalization;
using System.Windows.Data;

namespace Musicx.Ui.Core;

public class SecondsToTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double seconds)
        {
            var time = TimeSpan.FromSeconds(seconds);
            return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
        }
        
        if (value is long longSeconds)
        {
            var time = TimeSpan.FromSeconds(longSeconds);
            return $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}";
        }

        return "00:00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => throw new NotImplementedException();
}