using System.Collections.ObjectModel;
using Musicx.Application.Shared.Interfaces.Localization;

namespace Musicx.Presentation.Desktop.ViewModels.Pages;

public sealed class SettingsPageViewModel : ViewModelBase
{
    private readonly ITranslationService _translationService;

    public SettingsPageViewModel(ITranslationService translationService)
    {
        _translationService = translationService;
        Tabs =
        [
            Tr("Desktop.Settings.Playback", "Playback"),
            Tr("Desktop.Settings.Display", "Display"),
            Tr("Desktop.Settings.Import", "Import"),
            Tr("Desktop.Settings.Library", "Library"),
            Tr("Desktop.Settings.Network", "Network")
        ];
    }

    public ObservableCollection<string> Tabs { get; }

    private string Tr(string key, string fallback)
    {
        var value = _translationService[key];
        return value.StartsWith("!") ? fallback : value;
    }
}