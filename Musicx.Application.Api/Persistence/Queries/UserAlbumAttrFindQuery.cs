using Musicx.Application.API.Persistence.Filtering;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Enums;

namespace Musicx.Application.API.Persistence.Queries;

/// <summary>
/// Represents a query for filtering user album attributes.
/// </summary>
/// <remarks>
/// This record is used to encapsulate optional search criteria for <see cref="InUserAlbumAttribute"/> entities.
/// It implements <see cref="IFindQuery{T}"/> to allow standard query handling in repositories or services.
/// </remarks>
/// <since>0.6.8</since>
public record UserAlbumAttrFindQuery : IFindQuery<InUserAlbumAttribute>
{
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    public long? UserId { get; set; }
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? Album { get; set; }
    public TextFilter? Artist { get; set; }
    public TextFilter? Country { get; set; }
    public ICollection<ReleaseType>? ReleaseTypes { get; set; }
    public ICollection<long>? GenreIds { get; set; }
    public ICollection<long>? InfluenceIds { get; set; }
    public TextFilter? Label { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public TextFilter? Tag { get; set; }
    public TextFilter? User { get; set; }
}