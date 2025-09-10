namespace Musicx.Presentation.Web.Client.Helpers;
    
public sealed class DurationConverter : MudBlazor.Converter<long?>
{
    public DurationConverter()
    {
        // long -> string
        SetFunc = value =>
        {
            if (value is null)
            {
                return string.Empty;
            }
            
            var ts = TimeSpan.FromSeconds(value.Value);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
        };

        // string -> long
        GetFunc = text =>
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
        };
    }
}