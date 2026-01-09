using System.Text.Json.Nodes;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class AlbumGenreMapper
{
    public static OutAlbumGenre FromDicoToAlbumGenre(this IDictionary<string, object?> albumGenre) => new()
    {
        Album = albumGenre.FromDicoToAlbum(),
        Genre = albumGenre.FromDicoToGenre(),
        Tagger = albumGenre.FromDicoToUser(),
        
        CreatedAt = albumGenre.SafeGet<DateTime>(AlbumGenreColumns.CreatedAt),
        UpdatedAt = albumGenre.SafeGet<DateTime>(AlbumGenreColumns.UpdatedAt),
        
        Confidence = albumGenre.SafeGet<float>(AlbumGenreColumns.Confidence),
        Metadata = albumGenre.SafeGet<string?>(AlbumGenreColumns.Metadata),
        Source = albumGenre.SafeGet<GenreVoteSource>(AlbumGenreColumns.Source),
    };

    public static InAlbumGenre ToRaw(this OutAlbumGenre albumGenre) => new()
    {
        AlbumId = albumGenre.Album.Id,
        GenreId = albumGenre.Genre.Id,
        TaggerId = albumGenre.Tagger.Id,

        Confidence = albumGenre.Confidence,
        Metadata = albumGenre.Metadata,
        Source = albumGenre.Source,
    };
}