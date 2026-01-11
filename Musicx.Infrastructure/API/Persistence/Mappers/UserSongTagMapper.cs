using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserSongTagMapper
{
    public static OutUserSongTag FromDicoToUserSongTag(this IDictionary<string, object?> userSongTag) => new()
    {
        User = userSongTag.FromDicoToUser(),
        Song = userSongTag.FromDicoToSong(),
        Tag = userSongTag.FromDicoToTag(),

        CreatedAt = userSongTag.SafeGet<DateTime>(UserSongTagColumns.CreatedAt),
    };

    public static InUserSongTag ToRaw(this OutUserSongTag userSongTag) => new()
    {
        UserId = userSongTag.User.Id,
        SongId = userSongTag.Song.Id,
        TagId = userSongTag.Tag.Id,
    };
}