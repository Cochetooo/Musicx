using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Web.Interfaces.UseCases.User.FavoriteAlbum;

public interface IFindFavouriteAlbumByUserService
{
    Task<OutGenericList<OutUserFavoriteAlbum>> ExecuteAsync(long userId, CancellationToken cancellationToken = default);
    OutGenericList<OutUserFavoriteAlbum> Execute(long userId, CancellationToken cancellationToken = default);
}