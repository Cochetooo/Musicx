using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using System.Timers;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Musicx.Presentation.Web.Client.Modals.Searches;

public partial class SearchOverlay
{
    private ElementReference _wrapperRef;
    private string _inputId = $"search-input-{Guid.NewGuid().ToString("N").Substring(0, 6)}";
    private bool IsOpen { get; set; }
    private string _value = "";

    [Parameter] public string InitialValue { get; set; } = "";
    [Parameter] public EventCallback<string> OnClose { get; set; }
    [Parameter] public EventCallback<string> OnQuery { get; set; }

    private List<OutArtist> _artistResults = new();
    private List<OutAlbum> _albumResults = new();
    private List<OutUser> _userResults = new();
    private List<OutSong> _songResults = new();
    private List<OutGenre> _genreResults = new();
    
    private System.Timers.Timer _debounce;
    private CancellationTokenSource _cst;
    private bool _isLoading = false;
    private int _maxResults = 10;
    private string _sortBy = "relevance";
    private bool _groupResults = true;
    private bool _exactSearch = false;

    protected override void OnInitialized()
    {
        _value = InitialValue;
        _debounce = new System.Timers.Timer(250) { AutoReset = false };
        _debounce.Elapsed += async (_, _) => await InvokeAsync(DoSearchAsync);
    }

    public async Task OpenFromNavAsync(string sourceSelector)
    {
        await JS.InvokeVoidAsync("searchOverlay.openFromSelector", sourceSelector, $"#{_inputId}");
        IsOpen = true;
        StateHasChanged();
        await Task.Delay(120);
        await FocusInput();
    }
    
    private async Task FocusInput() => await JS.InvokeVoidAsync("searchOverlay.focusInput", $"#{_inputId}");
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
        
        _artistResults.Clear();
        _albumResults.Clear();
        _userResults.Clear();
        _songResults.Clear();
        _genreResults.Clear();
        
        _isLoading = false;
        StateHasChanged();
    }

    private void OnKeyDownHandler(KeyboardEventArgs ev)
    {
        if (ev.Key == "Escape")
        {
            _ = Close();
        }
        else if (ev.Key == "Enter")
        {
            _ = DoSearchAsync();
            OnQuery.InvokeAsync(_value);
        }
    }

    private void ApplyFilters() => _ = DoSearchAsync();
    
    
}