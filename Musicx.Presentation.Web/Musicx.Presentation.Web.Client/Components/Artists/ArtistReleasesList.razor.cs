using Microsoft.AspNetCore.Components;
using Musicx.Application.Shared.Utilities;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Artists;

public partial class ArtistReleasesList
{
    [Parameter] public IEnumerable<OutAlbum> Albums { get; set; } = [];
    [Parameter] public double Zoom { get; set; } = 1.0;
    [Parameter] public bool IsGrouped { get; set; } = true;
    [Parameter] public RatingMode RatingMode { get; set; }
    
    private Dictionary<ReleaseType, List<OutAlbum>> GroupedAlbums => 
        IsGrouped
            ? Albums.Where(a => a.ReleaseType.HasValue)
                .GroupBy(a => a.ReleaseType!.Value)
                .OrderBy(a => GetTypeOrder(a.Key))
                .ToDictionary(g => g.Key, g => g.ToList())
            : new() { { ReleaseType.Lp, Albums.ToList() }};

    int GetTypeOrder(ReleaseType? t)
    {
        if (t == ReleaseType.Lp) return 0;
        if (t == ReleaseType.Ep) return 1;
        if (t == ReleaseType.Single) return 3;
        return 2;
    }
    
    private string GetSimplifiedGenreStyle(OutAlbum album)
        =>
            $"background: {album.SimplifiedGenreColor}; color: {(ColorHelper.IsColorLight(album.SimplifiedGenreColor!) 
                ? ColorHelper.DarkColor 
                : "white")};";
}