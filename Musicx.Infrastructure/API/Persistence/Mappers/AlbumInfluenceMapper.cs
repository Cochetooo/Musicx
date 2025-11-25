using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class AlbumInfluenceMapper
{
    public static OutAlbumInfluence FromDicoToAlbumInfluence(this IDictionary<string, object?> albumInfluence) => new()
    {
        Album = albumInfluence.FromDicoToAlbum(),
        Genre = albumInfluence.FromDicoToGenre(),
        Tagger = albumInfluence.FromDicoToUser(),
        
        CreatedAt = albumInfluence.SafeGet<DateTime>(AlbumInfluenceColumns.CreatedAt),
        UpdatedAt = albumInfluence.SafeGet<DateTime>(AlbumInfluenceColumns.UpdatedAt),
        
        Confidence = albumInfluence.SafeGet<float>(AlbumInfluenceColumns.Confidence),
        Metadata = albumInfluence.SafeGet<string?>(AlbumInfluenceColumns.Metadata),
        Source = albumInfluence.SafeGet<GenreVoteSource>(AlbumInfluenceColumns.Source),
    };

    public static InAlbumInfluence ToRaw(this OutAlbumInfluence albumInfluence) => new()
    {
        AlbumId = albumInfluence.Album.Id,
        GenreId = albumInfluence.Genre.Id,
        TaggerId = albumInfluence.Tagger.Id,

        Confidence = albumInfluence.Confidence,
        Metadata = albumInfluence.Metadata,
        Source = albumInfluence.Source,
    };
}