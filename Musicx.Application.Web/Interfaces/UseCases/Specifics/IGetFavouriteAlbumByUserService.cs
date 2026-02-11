using Musicx.Contracts.Dto.Responses.Specifics.Lists;
using Musicx.Contracts.Dto.Responses.User;

namespace Musicx.Application.Web.Interfaces.UseCases.Specifics;

public interface IGetFavouriteAlbumByUserService
{
    Task<OutGenericList<OutUserFavoriteAlbum>> ExecuteAsync(long userId, CancellationToken cancellationToken = default);
    OutGenericList<OutUserFavoriteAlbum> Execute(long userId, CancellationToken cancellationToken = default);
}