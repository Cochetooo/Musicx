using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.Song;

public interface ISongRepository : IRepository<InSong, OutSong>
{
    Task<List<OutSong>> FindByAlbumIdAsync(long albumId,
        IQuerySpecification<InSong>? songQuerySpecification = null);
}