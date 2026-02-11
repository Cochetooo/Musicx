using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserFavoriteAlbumMapper
{
    public static OutUserFavoriteAlbum FromDicoToUserFavouriteAlbum
        (this IDictionary<string, object?> userFavAlbum) => new()
    {
        User = userFavAlbum.FromDicoToUser(),
        Album = userFavAlbum.FromDicoToAlbum(),
        
        CreatedAt = userFavAlbum.SafeGet<DateTime>(UserFavoriteAlbumColumns.CreatedAt),
        UpdatedAt = userFavAlbum.SafeGet<DateTime>(UserFavoriteAlbumColumns.UpdatedAt),
        
        Note = userFavAlbum.SafeGet<string?>(UserFavoriteAlbumColumns.Note),
        Order = userFavAlbum.SafeGet<short>(UserFavoriteAlbumColumns.Order),
    };

    public static InUserFavoriteAlbum ToRaw(this OutUserFavoriteAlbum userFavAlbum) => new()
    {
        UserId = userFavAlbum.User.Id,
        AlbumId = userFavAlbum.Album.Id,
        
        Note = userFavAlbum.Note,
        Order = userFavAlbum.Order
    };
}