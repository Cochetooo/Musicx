using Microsoft.AspNetCore.Components;
using MudBlazor;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Presentation.Web.Client.Components.GenreDetails;

public partial class GenreDetails
{
    [Parameter, EditorRequired] public OutAlbum Album { get; set; } = null!;
    [Parameter] public Size Size { get; set; }

    [Parameter] public bool DetailedView { get; set; }
    [Parameter] public EventCallback<bool> DetailedViewChanged { get; set; }

    private bool _isLoading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        if (Album.PrimaryGenres is null)
        {
            var result = await UcGetAlbum.ExecuteAsync(Album.Id, "genre");

            if (result is not null)
            {
                Album.PrimaryGenres = result.PrimaryGenres;
                Album.InfluenceGenres = result.InfluenceGenres;
            }
        }

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private string GetSimplifiedGenreStyle(OutAlbum album)
        =>
            $"background: {album.SimplifiedGenreColor}; color: {(ColorHelper.IsColorLight(album.SimplifiedGenreColor!) 
                ? ColorHelper.DarkColor 
                : "white")}; font-weight: bold";
}