using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Contracts.Dto.Requests.User;
using Musicx.Contracts.Dto.Responses.User;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Columns.User;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserAlbumAttrMapper
{
    public static OutUserAlbumAttribute FromDicoToUserAlbumAttr
        (this IDictionary<string, object?> userAlbumAttr) => new()
    {
        User = userAlbumAttr.FromDicoToUser(),
        Album = userAlbumAttr.FromDicoToAlbum(),

        CreatedAt = userAlbumAttr.SafeGet<DateTime>(UserAlbumAttrColumns.CreatedAt),
        UpdatedAt = userAlbumAttr.SafeGet<DateTime>(UserAlbumAttrColumns.UpdatedAt),

        CollectionType = userAlbumAttr.SafeGet<CollectionType?>(UserAlbumAttrColumns.CollectionType),
        DiscoveryDate = userAlbumAttr.SafeGet<DateTime?>(UserAlbumAttrColumns.DiscoveryDate),
        Rating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.Rating),
        ProductionRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.ProductionRating),
        LyricsRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.LyricsRating),
        InstrumentationRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.InstrumentationRating),
        VocalsRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.VocalsRating),
        AtmosphereRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.AtmosphereRating),
        OriginalityRating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.OriginalityRating),
        Review = userAlbumAttr.SafeGet<string?>(UserAlbumAttrColumns.Review),
        ReviewPostedAt = userAlbumAttr.SafeGet<DateTime?>(UserAlbumAttrColumns.ReviewPostedAt),
        ReviewSource = userAlbumAttr.SafeGet<long?>(ReviewSourceColumns.Id) is null ? null : new OutReviewSource
        {
            Id = userAlbumAttr.SafeGet<long>(ReviewSourceColumns.Id),
            Name = userAlbumAttr.SafeGet<string>(ReviewSourceColumns.Name) ?? string.Empty,
            Url = userAlbumAttr.SafeGet<string>(ReviewSourceColumns.Url) ?? string.Empty,
            Color = userAlbumAttr.SafeGet<string>(ReviewSourceColumns.Color) ?? string.Empty
        }
    };

    public static InUserAlbumAttribute ToRaw(this OutUserAlbumAttribute userAlbumAttr) => new()
    {
        UserId = userAlbumAttr.User.Id,
        AlbumId = userAlbumAttr.Album.Id,

        CollectionType = userAlbumAttr.CollectionType,
        DiscoveryDate = userAlbumAttr.DiscoveryDate,
        Rating = userAlbumAttr.Rating,
        ProductionRating = userAlbumAttr.ProductionRating,
        LyricsRating = userAlbumAttr.LyricsRating,
        InstrumentationRating = userAlbumAttr.InstrumentationRating,
        VocalsRating = userAlbumAttr.VocalsRating,
        AtmosphereRating = userAlbumAttr.AtmosphereRating,
        OriginalityRating = userAlbumAttr.OriginalityRating,
        Review = userAlbumAttr.Review,
        ReviewPostedAt = userAlbumAttr.ReviewPostedAt,
        ReviewSourceId = userAlbumAttr.ReviewSource?.Id
    };
}