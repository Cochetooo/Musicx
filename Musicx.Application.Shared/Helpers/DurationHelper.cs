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
    
    public static string FormatYearMonthDayDuration(DateTime from, DateTime to)
    {
        int years = to.Year - from.Year;
        int months = to.Month - from.Month;
        int days = to.Day - from.Day;

        if (days < 0)
        {
            months--;
            days += DateTime.DaysInMonth(to.AddMonths(-1).Year, to.AddMonths(-1).Month);
        }

        if (months < 0)
        {
            years--;
            months += 12;
        }

        var parts = new List<string>();

        if (years > 0)
            parts.Add($"{years} year{(years > 1 ? "s" : "")}");

        if (months > 0)
            parts.Add($"{months} month{(months > 1 ? "s" : "")}");

        if (days > 0 && years == 0)
            parts.Add($"{days} day{(days > 1 ? "s" : "")}");

        if (parts.Count == 0)
            return "today";

        if (parts.Count == 1)
            return parts[0];

        return string.Join(", ", parts.Take(parts.Count - 1)) + " and " + parts.Last();
    }
}