using Musicx.Contracts.Dto.Responses.Genre;
using Musicx.Contracts.Dto.Responses.Specifics.Artists;
using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.Specifics.Ratings;

namespace Musicx.Contracts.Dto.Responses.Specifics.Genres;

public sealed class OutGenreDataView
{
    public OutGenre Genre { get; set; } = null!;
    public OutGenericList<OutArtist> Artists { get; set; } = new();
    public OutAlbumList TopAlbums { get; set; } = new();
    public IReadOnlyList<OutArtistAlbumSummary> TopArtists { get; set; } = [];
    public IReadOnlyList<OutUserYearlyRating> YearlyRatings { get; set; } = [];
    public decimal? AlbumsAverageRating { get; set; }
}