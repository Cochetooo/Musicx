using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class SearchResult
{
    [Parameter] public string? SearchText { get; set; }

    private List<OutArtist> _artistsResult = [];
    private List<OutAlbum> _albumsResult = [];

    private int _artistSkip = 0;
    private int _albumSkip = 0;
    
    private readonly IReadOnlyList<BreadcrumbItem> _breadcrumb = 
    [
        new("Musicx", href: "/"),
        new("Search Results", href: "#")
    ];

    protected override async Task OnParametersSetAsync()
    {
        await SearchArtists();
        await SearchAlbums();
    }

    private async Task SearchArtists()
    {
        _artistsResult = await UcListArtist.ExecuteAsync(
            take: 10,
            skip: _artistSkip,
            filter: SearchText
        );
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task SearchAlbums()
    {
        _albumsResult = await UcListAlbum.ExecuteAsync(
            take: 10,
            skip: _albumSkip,
            filter: SearchText,
            query: "artist"
        );
        
        await InvokeAsync(StateHasChanged);
    }

    private async Task PreviousArtists()
    {
        _artistSkip -= 10;
        if (_artistSkip < 0)
        {
            _artistSkip = 0;
        }
        
        await SearchArtists();
    }

    private async Task NextArtists()
    {
        _artistSkip += 10;
        await SearchArtists();
    }

    private async Task PreviousAlbums()
    {
        _albumSkip -= 10;
        if (_albumSkip < 0)
        {
            _albumSkip = 0;
        }
        
        await SearchAlbums();
    }

    private async Task NextAlbums()
    {
        _albumSkip += 10;
        await SearchAlbums();
    } 
}