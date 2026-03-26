using System.Globalization;

namespace Musicx.Application.Shared.Interfaces.Localization;

public interface ITranslationService
{
    CultureInfo CurrentCulture { get; }
    string this[string key] { get; }
    string Get(string key, params object[] args);
    void SetCulture(CultureInfo culture);
}