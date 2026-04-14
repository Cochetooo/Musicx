using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;

namespace Musicx.Presentation.Web.Client.Components.Albums;

public partial class AlbumGridView
{
    [Parameter] public IEnumerable<OutAlbum> Albums { get; set; } = [];
    [Parameter] public double Zoom { get; set; } = 1.0;
    [Parameter] public bool IsGrouped { get; set; } = true;
    [Parameter] public RatingMode? RatingMode { get; set; }
    [Parameter] public bool ShowRank { get; set; }
    [Parameter] public bool ShowRating { get; set; }
    [Parameter] public bool ShowRatingCount { get; set; }
    
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
}