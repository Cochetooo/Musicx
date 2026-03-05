using MudBlazor;

namespace Musicx.Presentation.Web.Client.Helpers;
    
public sealed class DurationConverter : IConverter<long?, string?>
{
    public string? Convert(long? value)
    {
        if (value is null)
        {
            return string.Empty;
        }
            
        var ts = TimeSpan.FromSeconds(value.Value);
        return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
    }

    public long? ConvertBack(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var parts = text.Split(':');
        if (parts.Length != 2) return 0;

        if (int.TryParse(parts[0], out var minutes) &&
            int.TryParse(parts[1], out var seconds))
        {
            return minutes * 60 + seconds;
        }

        return 0;
    }
}