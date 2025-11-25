using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserArtistAttrMapper
{
    public static OutUserArtistAttribute FromDicoToUserArtistAttr
        (this IDictionary<string, object?> userArtistAttr) => new()
    {
        User = userArtistAttr.FromDicoToUser(),
        Artist = userArtistAttr.FromDicoToArtist(),

        CreatedAt = userArtistAttr.SafeGet<DateTime>(UserArtistAttrColumns.CreatedAt),
        UpdatedAt = userArtistAttr.SafeGet<DateTime>(UserArtistAttrColumns.UpdatedAt),

        Follow = userArtistAttr.SafeGet<bool?>(UserArtistAttrColumns.Follow),
        Rating = userArtistAttr.SafeGet<short?>(UserArtistAttrColumns.Rating),
    };

    public static InUserArtistAttribute ToRaw(this OutUserArtistAttribute userArtistAttr) => new()
    {
        UserId = userArtistAttr.User.Id,
        ArtistId = userArtistAttr.Artist.Id,

        Follow = userArtistAttr.Follow,
        Rating = userArtistAttr.Rating,
    };
}