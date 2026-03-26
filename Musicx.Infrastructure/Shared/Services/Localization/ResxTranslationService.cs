using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Musicx.Application.Shared.Interfaces.Localization;

namespace Musicx.Infrastructure.Shared.Services.Localization;

public sealed class ResxTranslationService : ITranslationService
{
    private readonly ResourceManager _resourceManager = new("Musicx.Infrastructure.Shared.Resources.Translations",
        typeof(ResxTranslationService).Assembly);

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

    public string this[string key] => _resourceManager.GetString(key, CurrentCulture) ?? $"!{key}!";

    public string Get(string key, params object[] args)
    {
        var format = this[key];
        return args.Length == 0 ? format : string.Format(CurrentCulture, format, args);
    }

    public void SetCulture(CultureInfo culture)
    {
        if (Equals(CurrentCulture, culture))
        {
            return;
        }

        CurrentCulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}