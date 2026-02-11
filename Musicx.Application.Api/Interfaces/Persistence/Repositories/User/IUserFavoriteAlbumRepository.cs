using Musicx.Application.Shared.Interfaces.Persistence;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Api.Interfaces.Persistence.Repositories.User;

public interface IUserFavoriteAlbumRepository : IRepository<InUserFavoriteAlbum, OutUserFavoriteAlbum>
{
    Task<IReadOnlyList<OutUserFavoriteAlbum>> FindByUserAsync(long userId);
}