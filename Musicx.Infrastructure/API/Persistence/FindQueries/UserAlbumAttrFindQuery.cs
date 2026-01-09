using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Infrastructure.API.Persistence.FindQueries;

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
    public string? Album { get; set; }
    public string? Artist { get; set; }
    public ICollection<long>? GenreIds { get; set; }
    public ICollection<long>? InfluenceIds { get; set; }
    public string? Label { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public string? Tag { get; set; }
    public string? User { get; set; }
}