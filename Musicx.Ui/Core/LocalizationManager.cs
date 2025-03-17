using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Markup;

namespace Musicx.Ui.Core;

public static class LocalizationManager
{
    private static readonly ResourceManager ResourceManager = new ResourceManager("Musicx.Ui.Locales.Translations", typeof(LocalizationManager).Assembly);

    public static void ChangeLanguage(string cultureCode)
    {
        var culture = new CultureInfo(cultureCode);
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;

        foreach (Window window in Application.Current.Windows)
        {
            window.Language = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
        }
    }

    public static string GetString(string key)
        => ResourceManager.GetString(key, Thread.CurrentThread.CurrentUICulture) ?? key;
}