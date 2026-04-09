using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Musicx.Application.Shared.Interfaces.Localization;
using Musicx.Infrastructure.Shared.Resources;

namespace Musicx.Infrastructure.Shared.Services.Localization;

public sealed class ResxTranslationService : ITranslationService
{
    private readonly ResourceManager _resourceManager = Translations.ResourceManager;

    public event PropertyChangedEventHandler? PropertyChanged;

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentUICulture;

    public string this[string key]
    {
        get
        {
            try
            {
                return _resourceManager.GetString(key, CurrentCulture) ?? $"!{key}!";
            }
            catch (MissingManifestResourceException)
            {
                return $"!{key}!";
            }
        }
    }

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