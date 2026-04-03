using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.Song;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Shared.Interfaces.UseCases.Song;

public interface IFindSongByAlbumService
{
    Task<List<OutSong>> ExecuteAsync(
        long albumId,
        IJoinSpecification<InSong>? joins = null,
        OrderSpecification<InSong>? order = null
    );
    
    List<OutSong> Execute(
        long albumId,
        IJoinSpecification<InSong>? joins = null,
        OrderSpecification<InSong>? order = null
    );
}