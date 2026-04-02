using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.User;

namespace Musicx.Application.API.Persistence.Queries;

public record UserArtistAttrFindQuery : IFindQuery<InUserArtistAttribute>
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }
}