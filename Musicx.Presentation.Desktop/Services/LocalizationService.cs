using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.Services;

public interface ILocalizationService : INotifyPropertyChanged
{
    string this[string key] { get; }
    CultureInfo CurrentCulture { get; }
    event EventHandler? LanguageChanged;
}

public sealed class LocalizationService : ReactiveObject, ILocalizationService
{
    private readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        ["en"] = new()
        {
            ["Title"] = "Musicx"
        }
    };

    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (!Equals(_currentCulture, value))
            {
                this.RaiseAndSetIfChanged(ref _currentCulture, value);
                LanguageChanged?.Invoke(this, EventArgs.Empty);
                this.RaisePropertyChanged("Item[]");
            }
        }
    }
    
    public event EventHandler? LanguageChanged;

    public string this[string key]
    {
        get
        {
            var lang = _currentCulture.TwoLetterISOLanguageName;
            return _translations.TryGetValue(lang, out var dict) && dict.TryGetValue(key, out var value) 
                ? value 
                : $"!{key}!";
        }
    }
}