using Musicx.Contracts.Dto.Responses.Specifics.Ratings;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Contracts.Dto.Responses.Specifics.Users;

/// <summary>
/// Aggregated payload for User page rendering.
/// </summary>
/// <since>0.7.4</since>
public sealed class OutUserDataView
{
    /// <summary>Main user model.</summary>
    public OutUser User { get; set; } = null!;

    /// <summary>User rating distribution and statistics.</summary>
    public OutUserRatingStats RatingStats { get; set; } = new();

    /// <summary>Top genres computed for user preferences.</summary>
    public IReadOnlyList<OutUserGenreRating> TopGenres { get; set; } = [];

    /// <summary>Top-rated/favorite albums for profile highlights.</summary>
    public IReadOnlyList<OutAlbum> FavoriteAlbums { get; set; } = [];
}