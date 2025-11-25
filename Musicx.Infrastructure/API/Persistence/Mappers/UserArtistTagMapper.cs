using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserArtistTagMapper
{
    public static OutUserArtistTag FromDicoToUserArtistTag(this IDictionary<string, object?> userArtistTag) => new()
    {
        User = userArtistTag.FromDicoToUser(),
        Artist = userArtistTag.FromDicoToArtist(),
        Tag = userArtistTag.FromDicoToTag(),

        CreatedAt = userArtistTag.SafeGet<DateTime>(UserArtistTagColumns.CreatedAt),
    };

    public static InUserArtistTag ToRaw(this OutUserArtistTag userArtistTag) => new()
    {
        UserId = userArtistTag.User.Id,
        ArtistId = userArtistTag.Artist.Id,
        TagId = userArtistTag.Tag.Id,
    };
}