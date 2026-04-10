using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Genre;

namespace Musicx.Application.API.Persistence.Queries;

public record GenreFindQuery : IFindQuery<InGenre>
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }
    
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public bool? IsVisible { get; set; }
}