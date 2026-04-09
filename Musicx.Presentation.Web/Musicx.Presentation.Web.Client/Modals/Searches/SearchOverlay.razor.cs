using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Models.Queries;
using Musicx.Contracts.Dto.Requests.Album;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Requests.Genre;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.Artist;
using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Specifications.Album;
using Musicx.Infrastructure.API.Persistence.Specifications.Song;

namespace Musicx.Presentation.Web.Client.Modals.Searches;

public partial class SearchOverlay : IAsyncDisposable
{
    private ElementReference _wrapperRef;
    private MudTextField<string> _mudInput = null!;
    private string _inputId = $"search-input-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
    private bool IsOpen { get; set; }
    private string _value = "";
    
    [Parameter] public EventCallback<string> OnClose { get; set; }
    [Parameter] public EventCallback<string> OnQuery { get; set; }

    private List<OutArtist> _artistResults = [];
    private List<OutAlbum> _albumResults = [];
    private List<OutUser> _userResults = [];
    private List<OutSong> _songResults = [];
    private List<OutGenre> _genreResults = [];
    
    private Dictionary<string, List<SearchItem>> _searchResults = [];
    
    private CancellationTokenSource? _cts;
    private bool _isLoading;
    private int _maxResults = 10;
    private string _sortBy = "relevance";
    private bool _groupResults = true;
    private bool _exactSearch;

    private IJSObjectReference? _searchOverlayModule;

    public async Task OpenFromNavAsync(string sourceSelector)
    {
        IsOpen = true;
        StateHasChanged();
        
        await Task.Delay(80);

        var targetSelector = ".f-search-input-mud input";
        _searchOverlayModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/searchOverlay.js");
        await _searchOverlayModule.InvokeVoidAsync("openFromSelector", sourceSelector, targetSelector);
        await _searchOverlayModule.InvokeVoidAsync("focusInput", targetSelector);
    }
    
    private void OnBackdropClick() => _ = Close();

    public async Task Close()
    {
        if (!IsOpen)
        {
            return;
        }

        _searchOverlayModule ??= await JS.InvokeAsync<IJSObjectReference>("import", "/Js/searchOverlay.js");
        await _searchOverlayModule.InvokeVoidAsync("closeOverlay");
        
        IsOpen = false;
        await OnClose.InvokeAsync(_value);
        
        ClearResults();

        _value = "";
        
        _isLoading = false;
        StateHasChanged();
    }

    private void ClearResults()
    {
        _artistResults.Clear();
        _albumResults.Clear();
        _userResults.Clear();
        _songResults.Clear();
        _genreResults.Clear();
        
        _searchResults.Clear();
    }

    private async Task OnKeyDownHandler(KeyboardEventArgs ev)
    {
        // Avoid special keys
        if (ev.CtrlKey || ev.AltKey || ev.MetaKey)
        {
            return;
        }
        
        if (ev.Key == "Escape")
        {
            await Close();
        }
        else if (ev.Key == "Enter" || ev.Key == " " || ev.Key == "Spacebar"
                 || (ev.Key.Length == 1 && char.IsLetterOrDigit(ev.Key[0])))
        {
            await DoSearchAsync();
        }
    }

    private async Task DoSearchAsync()
    {
        if (_cts is not null)
        {
            await _cts.CancelAsync();
        }
        
        _cts = new CancellationTokenSource();
        var token = _cts.Token;
        try
        {
            await Task.Delay(300, token);
            
            _isLoading = true;
            ClearResults();
            await InvokeAsync(StateHasChanged);

            var pagingOptions = new PagingOptions(_maxResults, 0);
            
            _artistResults = (await Api.FindAsync<InArtist, OutArtist>(
                pagingOptions: pagingOptions,
                query: FindQuery<InArtist>.Create(filter: _value, exact: _exactSearch)
            )).Items;

            _albumResults = (await Api.FindAsync<InAlbum, OutAlbum>(
                pagingOptions: pagingOptions,
                query: FindQuery<InAlbum>.Create(filter: _value, exact: _exactSearch),
                joins: new AlbumJoinSpecification
                {
                    IncludeArtist = true
                }
            )).Items;

            _genreResults = (await Api.FindAsync<InGenre, OutGenre>(
                pagingOptions: pagingOptions,
                query: FindQuery<InGenre>.Create(filter: _value, exact: _exactSearch)
            )).Items;

            _songResults = (await Api.FindAsync<InSong, OutSong>(
                pagingOptions: pagingOptions,
                query: FindQuery<InSong>.Create(filter: _value, exact: _exactSearch),
                joins: new SongJoinSpecification
                {
                    IncludeAlbum = true
                }
            )).Items;

            _userResults = (await Api.FindAsync<InUser, OutUser>(
                pagingOptions: pagingOptions,
                query: FindQuery<InUser>.Create(filter: _value, exact: _exactSearch)
            )).Items;

            if (_groupResults)
            {
                _searchResults.Add("Artist", _artistResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "Artist",
                        Country = a.CurrentCountry,
                        Image = a.ArtworkUrl,
                        Icon = GetIconForCategory("Artist"),
                        Subtitle = a.CalculatedGenres ?? "",
                        Title = a.Name
                    }).ToList());
                
                _searchResults.Add("Album", _albumResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "Album",
                        Image = a.ArtworkUrl,
                        Icon = GetIconForCategory("Album"),
                        Subtitle = a.Artist?.Name ?? "",
                        Title = a.Name
                    }).ToList());
                
                _searchResults.Add("Genre", _genreResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "Genre",
                        Icon = GetIconForCategory("Genre"),
                        Subtitle = a.Type.ToString(),
                        Title = a.CanonicalName
                    }).ToList());
                
                _searchResults.Add("User", _userResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "User",
                        Country = a.OriginCountry,
                        Image = a.PictureUrl,
                        Icon = GetIconForCategory("User"),
                        Subtitle = a.BirthDate?.ToString("dd-MM-yyyy") ?? "",
                        Title = a.Name
                    }).ToList());
                
                _searchResults.Add("Song", _songResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "Song",
                        Image = a.Album?.ArtworkUrl,
                        Icon = GetIconForCategory("Song"),
                        Subtitle = a.Album?.Name ?? "",
                        Title = a.Title
                    }).ToList());
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task OnResultClick(SearchItem item)
    {
        await Close();
        _value = "";
        Navigation.NavigateTo($"/{item.Category}/{item.Id}");
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_searchOverlayModule is not null)
        {
            await _searchOverlayModule.DisposeAsync();
        }

        _cts?.Dispose();
    }

    private string GetIconForCategory(string category) => category switch
        {
            "Artist" => "🎤",
            "Album" => "💿",
            "Song" => "🎵",
            "User" => "👤",
            "Genre" => "🎹",
            "Localization" => "🌎",
            "Label" => "🏷️",
            _ => "•"
        };

    public class SearchItem
    {
        public long Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Country { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}