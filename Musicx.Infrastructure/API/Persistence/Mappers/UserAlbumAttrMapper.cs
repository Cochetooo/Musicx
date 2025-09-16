using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class UserAlbumAttrMapper
{
    public static OutUserAlbumAttribute FromDicoToUserAlbumAttr
        (this IDictionary<string, object?> userAlbumAttr) => new()
    {
        UserId = userAlbumAttr.SafeGet<long>(UserAlbumAttrColumns.UserId),
        AlbumId = userAlbumAttr.SafeGet<long>(UserAlbumAttrColumns.AlbumId),

        CreatedAt = userAlbumAttr.SafeGet<DateTime>(UserAlbumAttrColumns.CreatedAt),
        UpdatedAt = userAlbumAttr.SafeGet<DateTime>(UserAlbumAttrColumns.UpdatedAt),

        CollectionType = userAlbumAttr.SafeGet<CollectionType?>(UserAlbumAttrColumns.CollectionType),
        Rating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.Rating),
        Review = userAlbumAttr.SafeGet<string?>(UserAlbumAttrColumns.Review)
    };

    public static InUserAlbumAttribute ToRaw(this OutUserAlbumAttribute userAlbumAttr) => new()
    {
        UserId = userAlbumAttr.UserId,
        AlbumId = userAlbumAttr.AlbumId,

        CollectionType = userAlbumAttr.CollectionType,
        Rating = userAlbumAttr.Rating,
        Review = userAlbumAttr.Review
    };
}