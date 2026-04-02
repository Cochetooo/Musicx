using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Artist;

namespace Musicx.Application.API.Persistence.Queries;

public record ArtistFindQuery : IFindQuery<InArtist>
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }
}