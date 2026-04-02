using Musicx.Contracts.Dto.Responses.Specifics.Lists;

namespace Musicx.Contracts.Dto.Responses.Specifics.Albums;

/// <summary>
/// Aggregated payload for Album page rendering.
/// </summary>
/// <since>0.7.4</since>
public sealed class OutAlbumDataView
{
    /// <summary>Main album model.</summary>
    public OutAlbum Album { get; set; } = null!;

    /// <summary>Album tracks ordered for display.</summary>
    public IReadOnlyList<OutSong> Songs { get; set; } = [];

    /// <summary>Previous release in artist discography order.</summary>
    public OutAlbum? PreviousAlbum { get; set; }

    /// <summary>Next release in artist discography order.</summary>
    public OutAlbum? NextAlbum { get; set; }

    /// <summary>Current user attribute for this album, when available.</summary>
    public OutUserAlbumAttribute? CurrentUserAttribute { get; set; }

    /// <summary>Global ratings summary list (paged/limited by builder).</summary>
    public OutGenericList<OutUserAlbumAttribute> Ratings { get; set; } = new();
}