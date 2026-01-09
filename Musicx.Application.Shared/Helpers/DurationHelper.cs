namespace Musicx.Application.Shared.Helpers;

public static class DurationHelper
{
    public static string FormatDuration(this long? totalSeconds)
    {
        if (totalSeconds is null)
        {
            return "-:--";
        }
        
        bool negative = totalSeconds < 0;
        var ts = TimeSpan.FromSeconds(Math.Abs(totalSeconds.Value));

        string result;

        if (ts.TotalHours >= 1)
        {
            long hours = (long)ts.TotalHours;
            result = $"{hours}:{ts.Minutes:D2}:{ts.Seconds:D2}";
        }
        else
        {
            result = ts.Minutes >= 10 
                ? $"{ts.Minutes:D2}:{ts.Seconds:D2}" 
                : $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        return negative ? "-" + result : result;
    }
}