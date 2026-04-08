using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Enums;

namespace Musicx.Application.API.Persistence.Queries;

public record ArtistFindQuery : IFindQuery<InArtist>
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }

    public ChartType? ChartType { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public short? MinUserAge { get; set; }
    public short? MaxUserAge { get; set; }
    public short? PopularityWeight { get; set; }
    public ArtistDiscriminator? Discriminator { get; set; }
    public string? Country { get; set; }
    public long? MainGenreId { get; set; }
    public long[]? PrimaryGenreIds { get; set; }
    public long[]? InfluenceGenreIds { get; set; }
}