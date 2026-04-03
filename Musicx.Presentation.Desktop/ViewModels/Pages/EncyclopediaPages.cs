using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Localization;
using Musicx.Application.Shared.Interfaces.UseCases;
using Musicx.Application.Shared.Interfaces.UseCases.Security;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.Specifics;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.User;
using ReactiveUI;

namespace Musicx.Presentation.Desktop.ViewModels.Pages;

public abstract class TranslatableViewModel(ITranslationService translationService) : ViewModelBase
{
    protected string T(string key) => translationService[key];
    protected string Tx(string key, string fallback)
    {
        var v = T(key);
        return v.StartsWith("!") ? fallback : v;
    }
}

public sealed class ArtistPageViewModel(
    IFindAllService<InArtist, OutArtist> ucFind, 
    IFindInService<InArtist, OutArtist> ucFindIn,
    IFindOneByIdService<InArtist, OutArtist> ucFindOneById,
    ITranslationService t)
    : TranslatableViewModel(t)
{
    private string _filter = string.Empty;
    private long _selectedId;

    public ObservableCollection<OutArtist> Artists { get; } = [];

    public string Title => Tx("Web.ArtistView.Title", "Artist");
    public string SearchLabel => Tx("Web.Common.Search", "Search");
    public string FindLabel => Tx("Desktop.Common.Find", "Find");
    public string FindByIdLabel => Tx("Desktop.Common.FindById", "FindById");
    public string FindInLabel => Tx("Desktop.Common.FindIn", "FindIn");

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public long SelectedId
    {
        get => _selectedId;
        set => this.RaiseAndSetIfChanged(ref _selectedId, value);
    }

    public ReactiveCommand<Unit, Unit> FindCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        var list = await ucFind.ExecuteAsync(
            filter: Filter,
            pagingOptions: new PagingOptions(Take: 100, Skip: 0)
        );
        Replace(Artists, list.Items);
    });

    public ReactiveCommand<Unit, Unit> FindByIdCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var artist = await ucFindOneById.ExecuteAsync(id: SelectedId);
        Replace(Artists, artist is null ? [] : [artist]);
    });

    public ReactiveCommand<Unit, Unit> FindInCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var artists = await ucFindIn.ExecuteAsync(ids: [SelectedId]);
        Replace(Artists, artists);
    });

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }
}

public sealed class AlbumPageViewModel(
    IFindAllService<InAlbum, OutAlbum> ucFind, 
    IFindInService<InAlbum, OutAlbum> ucFindIn,
    IFindOneByIdService<InAlbum, OutAlbum> ucFindOneById,
    ITranslationService t)
    : TranslatableViewModel(t)
{
    private string _filter = string.Empty;
    private long _selectedId;
    private AlbumDisplayMode _displayMode = AlbumDisplayMode.Grid;

    public ObservableCollection<OutAlbum> Albums { get; } = [];

    public string Title => Tx("Web.AlbumView.Title", "Album");
    public string SearchLabel => Tx("Web.Common.Search", "Search");
    public string FindLabel => Tx("Desktop.Common.Find", "Find");
    public string FindByIdLabel => Tx("Desktop.Common.FindById", "FindById");
    public string FindInLabel => Tx("Desktop.Common.FindIn", "FindIn");
    public Array DisplayModes => Enum.GetValues<AlbumDisplayMode>();

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public long SelectedId
    {
        get => _selectedId;
        set => this.RaiseAndSetIfChanged(ref _selectedId, value);
    }

    public AlbumDisplayMode DisplayMode
    {
        get => _displayMode;
        set => this.RaiseAndSetIfChanged(ref _displayMode, value);
    }

    public ReactiveCommand<Unit, Unit> FindCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        var list = await ucFind.ExecuteAsync(filter: Filter, pagingOptions: new PagingOptions(Take: 100, Skip: 0));
        Replace(Albums, list.Items);
    });

    public ReactiveCommand<Unit, Unit> FindByIdCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var album = await ucFindOneById.ExecuteAsync(SelectedId);
        Replace(Albums, album is null ? [] : [album]);
    });

    public ReactiveCommand<Unit, Unit> FindInCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var albums = await ucFindIn.ExecuteAsync([SelectedId]);
        Replace(Albums, albums);
    });

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }
}

public sealed class GenrePageViewModel(
    IFindAllService<InGenre, OutGenre> ucFind, 
    IFindInService<InGenre, OutGenre> ucFindIn,
    IFindOneByIdService<InGenre, OutGenre> ucFindOneById,
    ITranslationService t)
    : TranslatableViewModel(t)
{
    private string _filter = string.Empty;
    private long _selectedId;

    public ObservableCollection<OutGenre> Genres { get; } = [];

    public string Title => Tx("Web.GenreView.Title", "Genre");
    public string SearchLabel => Tx("Web.Common.Search", "Search");
    public string FindLabel => Tx("Desktop.Common.Find", "Find");
    public string FindByIdLabel => Tx("Desktop.Common.FindById", "FindById");
    public string FindInLabel => Tx("Desktop.Common.FindIn", "FindIn");

    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public long SelectedId
    {
        get => _selectedId;
        set => this.RaiseAndSetIfChanged(ref _selectedId, value);
    }

    public ReactiveCommand<Unit, Unit> FindCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        var list = await ucFind.ExecuteAsync(filter: Filter, pagingOptions: new PagingOptions(Take: 100, Skip: 0));
        Replace(Genres, list.Items);
    });

    public ReactiveCommand<Unit, Unit> FindByIdCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var genre = await ucFindOneById.ExecuteAsync(SelectedId);
        Replace(Genres, genre is null ? [] : [genre]);
    });

    public ReactiveCommand<Unit, Unit> FindInCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var genres = await ucFindIn.ExecuteAsync([SelectedId]);
        Replace(Genres, genres);
    });

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }
}

public sealed class UserPageViewModel(
    IAuthSignInService ucSignIn,
    IFindAllService<InUser, OutUser> ucFind, 
    IFindInService<InUser, OutUser> ucFindIn,
    IFindOneByIdService<InUser, OutUser> ucFindOneById,
    ITranslationService t)
    : TranslatableViewModel(t)
{
    private string _filter = string.Empty;
    private long _selectedId;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _status = string.Empty;

    public ObservableCollection<OutUser> Users { get; } = [];

    public string Title => Tx("Web.UserView.Title", "User");
    public string SearchLabel => Tx("Web.Common.Search", "Search");
    public string FindLabel => Tx("Desktop.Common.Find", "Find");
    public string FindByIdLabel => Tx("Desktop.Common.FindById", "FindById");
    public string FindInLabel => Tx("Desktop.Common.FindIn", "FindIn");
    public string LoginLabel => Tx("Web.Login.SignIn", "Sign in");
    public string Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public string Filter { get => _filter; set => this.RaiseAndSetIfChanged(ref _filter, value); }
    public long SelectedId { get => _selectedId; set => this.RaiseAndSetIfChanged(ref _selectedId, value); }
    public string Email { get => _email; set => this.RaiseAndSetIfChanged(ref _email, value); }
    public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }

    public ReactiveCommand<Unit, Unit> LoginCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        var session = await ucSignIn.ExecuteAsync(new SignInRequest(Email, Password));
        Status = session is null ? Tx("Web.Login.Failed", "Sign in failed") : $"{Tx("Web.Login.Success", "Signed in")}: {session}";
    });

    public ReactiveCommand<Unit, Unit> FindCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        var list = await ucFind.ExecuteAsync(filter: Filter, pagingOptions: new PagingOptions(Take: 100, Skip: 0));
        Replace(Users, list.Items);
    });

    public ReactiveCommand<Unit, Unit> FindByIdCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var user = await ucFindOneById.ExecuteAsync(SelectedId);
        Replace(Users, user is null ? [] : [user]);
    });

    public ReactiveCommand<Unit, Unit> FindInCommand => ReactiveCommand.CreateFromTask(async () =>
    {
        if (SelectedId <= 0) return;
        var users = await ucFindIn.ExecuteAsync([SelectedId]);
        Replace(Users, users);
    });

    private static void Replace<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source) target.Add(item);
    }
}

public enum AlbumDisplayMode
{
    Timeline,
    Grid,
    List
}