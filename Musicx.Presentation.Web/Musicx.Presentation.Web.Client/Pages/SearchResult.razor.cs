using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Pages;

public partial class SearchResult
{
    [Parameter] public string? SearchText { get; set; }

    private List<OutArtist> _artistsResult = [];
    private List<OutAlbum> _albumsResult = [];
    
    private readonly IReadOnlyList<BreadcrumbItem> _breadcrumb = 
    [
        new("Musicx", href: "/"),
        new("Search Results", href: "#")
    ];

    protected override async Task OnParametersSetAsync()
    {
        await UpdateResults();
    }

    private async Task UpdateResults()
    {
        _artistsResult = await UcListArtist.ExecuteAsync(
            take: 10,
            filter: SearchText
        );
        
        await InvokeAsync(StateHasChanged);

        _albumsResult = await UcListAlbum.ExecuteAsync(
            take: 10,
            filter: SearchText,
            query: "artist"
        );
        
        await InvokeAsync(StateHasChanged);
    }
}