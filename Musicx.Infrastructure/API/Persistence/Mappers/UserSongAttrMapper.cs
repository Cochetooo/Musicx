using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserSongAttrMapper
{
    public static OutUserSongAttribute FromDicoToUserSongAttr
        (this IDictionary<string, object?> userSongAttr) => new()
    {
        User = userSongAttr.FromDicoToUser(),
        Song = userSongAttr.FromDicoToSong(),

        CreatedAt = userSongAttr.SafeGet<DateTime>(UserSongAttrColumns.CreatedAt),
        UpdatedAt = userSongAttr.SafeGet<DateTime>(UserSongAttrColumns.UpdatedAt),
        
        Rating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.Rating),
    };

    public static InUserSongAttribute ToRaw(this OutUserSongAttribute userSongAttr) => new()
    {
        UserId = userSongAttr.User.Id,
        SongId = userSongAttr.Song.Id,
        
        Rating = userSongAttr.Rating,
    };
}