using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using System.Timers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Musicx.Application.Desktop.Specifications;
using Musicx.Application.Shared.Enums;

namespace Musicx.Presentation.Web.Client.Modals.Searches;

public partial class SearchOverlay
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

    public async Task OpenFromNavAsync(string sourceSelector)
    {
        IsOpen = true;
        StateHasChanged();
        
        await Task.Delay(80);

        var targetSelector = ".f-search-input-mud input";
        await JS.InvokeVoidAsync("searchOverlay.openFromSelector", sourceSelector, targetSelector);
        await JS.InvokeVoidAsync("searchOverlay.focusInput", targetSelector);
    }
    
    private void OnBackdropClick() => _ = Close();

    public async Task Close()
    {
        if (!IsOpen)
        {
            return;
        }

        await JS.InvokeVoidAsync("searchOverlay.closeOverlay");
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

    private void ApplyFilters() => _ = DoSearchAsync();

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
            
            _artistResults = await UcListArtists.ExecuteAsync(
                pagingOptions: pagingOptions,
                filter: _value,
                filterExact: _exactSearch
            );

            _albumResults = await UcListAlbums.ExecuteAsync(
                pagingOptions: pagingOptions,
                filter: _value,
                filterExact: _exactSearch,
                joins: new AlbumJoinSpecification
                {
                    IncludeArtist = true
                }
            );

            _genreResults = await UcListGenres.ExecuteAsync(
                pagingOptions: pagingOptions,
                filter: _value,
                filterExact: _exactSearch
            );

            _songResults = await UcListSongs.ExecuteAsync(
                pagingOptions: pagingOptions,
                filter: _value,
                filterExact: _exactSearch,
                joins: new SongJoinSpecification
                {
                    IncludeAlbum = true
                }
            );

            _userResults = await UcListUsers.ExecuteAsync(
                pagingOptions: pagingOptions,
                filter: _value,
                filterExact: _exactSearch
            );

            if (_groupResults)
            {
                _searchResults.Add("Artist", _artistResults
                    .Select(a => new SearchItem
                    {
                        Id = a.Id,
                        Category = "Artist",
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

    private string GetIconForCategory(string category) => category switch
        {
            "Artist" => "🎤",
            "Album" => "💿",
            "Song" => "🎵",
            "User" => "👤",
            "Genre" => "🏷️",
            "Localization" => "📍",
            "Label" => "🏷️",
            _ => "•"
        };

    public class SearchItem
    {
        public long Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}