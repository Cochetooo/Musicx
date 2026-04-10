using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Application.Shared.Interfaces.Persistence.Filtering;
using Musicx.Contracts.Dto.Requests.Album;

namespace Musicx.Application.API.Persistence.Queries;

public record AlbumFindQuery : IFindQuery<InAlbum>
{
    public TextSearchFilterOptions? Search { get; set; }
    public TextFilter? RawSearch { get; set; }
    
    public DateTime? CreatedAtFrom { get; set; }
    public DateTime? CreatedAtTo { get; set; }
    public bool? IsVisible { get; set; }
}