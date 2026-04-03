using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
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
        ProductionRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.ProductionRating),
        LyricsRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.LyricsRating),
        InstrumentationRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.InstrumentationRating),
        VocalsRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.VocalsRating),
        AtmosphereRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.AtmosphereRating),
        OriginalityRating = userSongAttr.SafeGet<short?>(UserSongAttrColumns.OriginalityRating),
    };

    public static InUserSongAttribute ToRaw(this OutUserSongAttribute userSongAttr) => new()
    {
        UserId = userSongAttr.User.Id,
        SongId = userSongAttr.Song.Id,
        
        Rating = userSongAttr.Rating,
        ProductionRating = userSongAttr.ProductionRating,
        LyricsRating = userSongAttr.LyricsRating,
        InstrumentationRating = userSongAttr.InstrumentationRating,
        VocalsRating = userSongAttr.VocalsRating,
        AtmosphereRating = userSongAttr.AtmosphereRating,
        OriginalityRating = userSongAttr.OriginalityRating,
    };
}