using Microsoft.AspNetCore.Components;
using Musicx.Contracts.Dto.Responses;
using Musicx.Presentation.Web.Client.Components.Charts;

namespace Musicx.Presentation.Web.Client.Components.Albums;

public partial class AlbumTimelineView
{
    [Parameter] public IEnumerable<OutAlbum> Albums { get; set; } = [];

    private int YearTickInterval { get; set; } = 5;

    private IReadOnlyList<AlbumTimelineEntry> TimelineAlbums => Albums
        .Where(album => album.OriginalReleaseDate.HasValue && !string.IsNullOrWhiteSpace(album.ArtworkUrl))
        .Select(album => new AlbumTimelineEntry(
            album.Id,
            album.Name,
            album.OriginalReleaseDate!.Value,
            album.ArtworkUrl!,
            album.Stats?.Average ?? 0
        ))
        .OrderBy(album => album.OriginalReleaseDate)
        .ToList();

    private Task OnYearTickIntervalChanged(int interval)
    {
        YearTickInterval = interval;
        return Task.CompletedTask;
    }

    private void OnAlbumClicked(long albumId)
        => Navigation.NavigateTo($"/Album/{albumId}");
}