using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

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
        DiscoveryDate = userAlbumAttr.SafeGet<DateTime>(UserAlbumAttrColumns.DiscoveryDate),
        Rating = userAlbumAttr.SafeGet<short?>(UserAlbumAttrColumns.Rating),
        Review = userAlbumAttr.SafeGet<string?>(UserAlbumAttrColumns.Review)
    };

    public static InUserAlbumAttribute ToRaw(this OutUserAlbumAttribute userAlbumAttr) => new()
    {
        UserId = userAlbumAttr.User.Id,
        AlbumId = userAlbumAttr.Album.Id,

        CollectionType = userAlbumAttr.CollectionType,
        DiscoveryDate = userAlbumAttr.DiscoveryDate,
        Rating = userAlbumAttr.Rating,
        Review = userAlbumAttr.Review
    };
}