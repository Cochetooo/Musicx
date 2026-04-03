using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserSongAttrsRepository : IRepository<InUserSongAttribute, OutUserSongAttribute>
{
    Task DeleteAsync(long userId, long songId);
    Task<OutUserSongAttribute?> FindOneBySongUserAsync(long userId, long songId);
    Task<IReadOnlyList<OutUserSongAttribute>> FindByAlbumUserAsync(long userId, long albumId);
}