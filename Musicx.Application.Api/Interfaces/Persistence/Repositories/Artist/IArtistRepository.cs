using Musicx.Application.Shared.Enums;
using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Artist;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Artist;

public interface IArtistRepository : IRepository<InArtist, OutArtist>
{
    Task<IReadOnlyList<OutArtist>> FindByGenreIdAsync(
        long genreId, 
        PagingOptions? pagingOptions = null
    );

    Task<long> GetCountByGenreIdAsync(long genreId);
}