using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class ReleaseMapper
{
    public static OutRelease FromDicoToRelease(this IDictionary<string, object?> release) => new()
    {
        Id = release.SafeGet<long>(ReleaseColumns.Id),

        Album = release.FromDicoToAlbum(),
        Label = release.SafeGet<long?>(ReleaseColumns.LabelId) != null
            ? release.FromDicoToLabel()
            : null,

        CatalogNumber = release.SafeGet<string>(ReleaseColumns.CatalogNumber) ?? string.Empty,
        IsVisible = release.SafeGet<bool>(ReleaseColumns.IsVisible),
        ReleaseDate = release.SafeGet<DateTime?>(ReleaseColumns.ReleaseDate),
    };

    public static InRelease ToRaw(this OutRelease release) => new()
    {
        Id = release.Id,

        AlbumId = release.Album.Id,
        LabelId = release.Label?.Id,

        CatalogNumber = release.CatalogNumber,
        IsVisible = release.IsVisible,
        ReleaseDate = release.ReleaseDate,
    };
}