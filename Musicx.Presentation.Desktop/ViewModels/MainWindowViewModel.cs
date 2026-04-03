using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Musicx.Application.Shared.Interfaces.Localization;
using Musicx.Presentation.Desktop.ViewModels.Pages;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly ArtistPageViewModel _artistPage;
    private readonly AlbumPageViewModel _albumPage;
    private readonly GenrePageViewModel _genrePage;
    private readonly UserPageViewModel _userPage;
    private readonly LibraryPageViewModel _libraryPage;
    private readonly SettingsPageViewModel _settingsPage;

    private ViewModelBase _currentPage;
    public string AppTitle { get; }

    public MainWindowViewModel(
        ArtistPageViewModel artistPage,
        AlbumPageViewModel albumPage,
        GenrePageViewModel genrePage,
        UserPageViewModel userPage,
        LibraryPageViewModel libraryPage,
        SettingsPageViewModel settingsPage,
        BottomAudioPlayerViewModel player,
        ITranslationService translationService)
    {
        _artistPage = artistPage;
        _albumPage = albumPage;
        _genrePage = genrePage;
        _userPage = userPage;
        _libraryPage = libraryPage;
        _settingsPage = settingsPage;

        AppTitle = Tx(translationService, "Desktop.App.Title", "MusicX");
        ArtistNavLabel = Tx(translationService, "Desktop.Nav.Artist", "ArtistView");
        AlbumNavLabel = Tx(translationService, "Desktop.Nav.Album", "AlbumView");
        GenreNavLabel = Tx(translationService, "Desktop.Nav.Genre", "GenreView");
        UserNavLabel = Tx(translationService, "Desktop.Nav.User", "UserView");
        LibraryNavLabel = Tx(translationService, "Desktop.Nav.Library", "LibraryView");
        SettingsNavLabel = Tx(translationService, "Desktop.Nav.Settings", "Settings");

        Player = player;
        _currentPage = _artistPage;

        NavigateArtistCommand = ReactiveCommand.Create(() => CurrentPage = _artistPage);
        NavigateAlbumCommand = ReactiveCommand.Create(() => CurrentPage = _albumPage);
        NavigateGenreCommand = ReactiveCommand.Create(() => CurrentPage = _genrePage);
        NavigateUserCommand = ReactiveCommand.Create(() => CurrentPage = _userPage);
        NavigateLibraryCommand = ReactiveCommand.Create(() => CurrentPage = _libraryPage);
        NavigateSettingsCommand = ReactiveCommand.Create(() => CurrentPage = _settingsPage);
    }

    public BottomAudioPlayerViewModel Player { get; }

    public string ArtistNavLabel { get; }
    public string AlbumNavLabel { get; }
    public string GenreNavLabel { get; }
    public string UserNavLabel { get; }
    public string LibraryNavLabel { get; }
    public string SettingsNavLabel { get; }

    public ReactiveCommand<Unit, ViewModelBase> NavigateArtistCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> NavigateAlbumCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> NavigateGenreCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> NavigateUserCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> NavigateLibraryCommand { get; }
    public ReactiveCommand<Unit, ViewModelBase> NavigateSettingsCommand { get; }

    public ViewModelBase CurrentPage
    {
        get => _currentPage;
        private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    private static string Tx(ITranslationService t, string key, string fallback)
    {
        var value = t[key];
        return value.StartsWith("!") ? fallback : value;
    }
}

public sealed record NavigationItem(string Name, ViewModelBase Page);