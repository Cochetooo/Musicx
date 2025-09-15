using System.Collections.Immutable;
using System.Dynamic;
using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class AlbumMapper
{
    private static readonly JsonSerializerSettings GenreMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenre>("genre")
        }
    };

    public static OutAlbum FromDicoToAlbum(this IDictionary<string, object?> album) => new()
    {
        Id = album.SafeGet<long>(AlbumColumns.Id),

        CreatedAt = album.SafeGet<DateTime>(AlbumColumns.CreatedAt),
        UpdatedAt = album.SafeGet<DateTime>(AlbumColumns.UpdatedAt),

        Artist = album.SafeGet<long?>(ArtistColumns.Id) != null
            ? album.FromDicoToArtist()
            : null,
        
        ArtistId = album.SafeGet<long?>(AlbumColumns.ArtistId),

        Releases = album.TryGetValue("releases", out var releaseValue)
                   && releaseValue is not null
            ? JsonConvert.DeserializeObject<OutRelease[]>(releaseValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,

        PrimaryGenres = album.TryGetValue("primary_genres", out var primaryGenreValue)
                        && primaryGenreValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(primaryGenreValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,

        InfluenceGenres = album.TryGetValue("influence_genres", out var influenceGenreValue)
                          && influenceGenreValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(influenceGenreValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,

        ArtworkUrl = album.SafeGet<string>(AlbumColumns.ArtworkUrl),
        BeginRecordDate = album.SafeGet<DateTime?>(AlbumColumns.BeginRecordDate),
        DiscTotal = album.SafeGet<int>(AlbumColumns.DiscTotal),
        EndRecordDate = album.SafeGet<DateTime?>(AlbumColumns.EndRecordDate),
        IsFarRight = album.SafeGet<bool>(AlbumColumns.IsFarRight),
        IsNsfw = album.SafeGet<bool>(AlbumColumns.IsNsfw),
        Language = album.SafeGet<string?>(AlbumColumns.Language),
        Name = album.SafeGet<string>(AlbumColumns.Name) ?? "",
        OriginalReleaseDate = album.SafeGet<DateTime?>(AlbumColumns.OriginalReleaseDate),
        ReleaseType = album.SafeGet<ReleaseType?>(AlbumColumns.ReleaseType),
        SimplifiedGenreColor = album.SafeGet<string>(AlbumColumns.SimplifiedGenreColor),
        SimplifiedGenreName = album.SafeGet<string>(AlbumColumns.SimplifiedGenreName),
        TotalDuration = album.SafeGet<long?>(AlbumColumns.TotalDuration),
        TrackTotal = album.SafeGet<int?>(AlbumColumns.TrackTotal),
    };

    public static InAlbum ToRaw(this OutAlbum album) => new()
    {
        Id = album.Id,
        
        ArtistId = album.ArtistId,
        ReleaseIds = album.Releases?.Select(r => r.Id).ToList(),
        PrimaryGenreIds = album.PrimaryGenres?.Select(g => g.Id).ToList(),
        InfluenceGenreIds = album.InfluenceGenres?.Select(g => g.Id).ToList(),
        
        ArtworkUrl = album.ArtworkUrl,
        BeginRecordDate = album.BeginRecordDate,
        DiscTotal = album.DiscTotal,
        EndRecordDate = album.EndRecordDate,
        IsFarRight = album.IsFarRight,
        IsNsfw = album.IsNsfw,
        Language = album.Language,
        Name = album.Name,
        OriginalReleaseDate = album.OriginalReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };
}