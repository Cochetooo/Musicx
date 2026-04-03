using System.Reactive;
using Musicx.Application.Shared.Interfaces.Localization;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels.Pages;

public sealed class BottomAudioPlayerViewModel : ViewModelBase
{
    private readonly ITranslationService _translationService;
    private string _nowPlaying;

    public BottomAudioPlayerViewModel(ITranslationService translationService)
    {
        _translationService = translationService;
        _nowPlaying = Tr("Desktop.Player.NothingPlaying", "Nothing playing");
        Back30Command = ReactiveCommand.Create(() => { });
        PlayPauseCommand = ReactiveCommand.Create(() => { });
        Forward30Command = ReactiveCommand.Create(() => { });
        ToggleEqualizerCommand = ReactiveCommand.Create(() => { });
    }

    public string NowPlaying
    {
        get => _nowPlaying;
        set => this.RaiseAndSetIfChanged(ref _nowPlaying, value);
    }

    public ReactiveCommand<Unit, Unit> Back30Command { get; }
    public ReactiveCommand<Unit, Unit> PlayPauseCommand { get; }
    public ReactiveCommand<Unit, Unit> Forward30Command { get; }
    public ReactiveCommand<Unit, Unit> ToggleEqualizerCommand { get; }

    public string Back30Label => Tr("Desktop.Player.Back30", "⏪ 30s");
    public string PlayPauseLabel => Tr("Desktop.Player.PlayPause", "⏯");
    public string Forward30Label => Tr("Desktop.Player.Forward30", "30s ⏩");
    public string EqualizerLabel => Tr("Desktop.Player.Equalizer", "Equalizer");

    private string Tr(string key, string fallback)
    {
        var value = _translationService[key];
        return value.StartsWith("!") ? fallback : value;
    }
}