using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;

namespace Musicx.Application.Api.Interfaces.FindQueries;

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