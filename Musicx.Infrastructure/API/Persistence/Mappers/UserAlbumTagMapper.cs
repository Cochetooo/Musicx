using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserAlbumTagMapper
{
    public static OutUserAlbumTag FromDicoToUserAlbumTag(this IDictionary<string, object?> userAlbumTag) => new()
    {
        User = userAlbumTag.FromDicoToUser(),
        Album = userAlbumTag.FromDicoToAlbum(),
        Tag = userAlbumTag.FromDicoToTag(),

        CreatedAt = userAlbumTag.SafeGet<DateTime>(UserAlbumTagColumns.CreatedAt),
    };

    public static InUserAlbumTag ToRaw(this OutUserAlbumTag userAlbumTag) => new()
    {
        UserId = userAlbumTag.User.Id,
        AlbumId = userAlbumTag.Album.Id,
        TagId = userAlbumTag.Tag.Id,
    };
}