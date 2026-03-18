using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserArtistAttrsRepository : IRepository<InUserArtistAttribute, OutUserArtistAttribute>
{
    Task DeleteAsync(long userId, long artistId);
    Task<IReadOnlyList<OutUserArtistAttribute>> FindByArtistAsync(long artistId, bool followersOnly = true);
    Task<IReadOnlyList<OutUserArtistAttribute>> FindByUserAsync(long userId, bool followersOnly = true);
    Task<OutUserArtistAttribute?> FindOneAsync(long userId, long artistId);
    Task<long> CountFollowersByArtistAsync(long artistId);
}