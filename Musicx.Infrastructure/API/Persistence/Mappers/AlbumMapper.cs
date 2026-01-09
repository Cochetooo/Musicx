using System.Collections.Immutable;
using System.Dynamic;
using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Dto.Responses.Specifics.Genres;
using Musicx.Contracts.Enums;
using Musicx.Application.Shared.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Musicx.Infrastructure.API.Persistence.Converters;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class AlbumMapper
{
    private static readonly JsonSerializerSettings ReleaseMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutRelease>("release")
        }
    };
    
    private static readonly JsonSerializerSettings AlbumGenreJsonOptions = new()
    {
        Converters =
        {
            new AlbumGenreNodeConverter(),
            new JsonToOutModelConverter<OutGenre>("genre"),
            new JsonToOutModelConverter<OutAlbumGenre>("album_genre")
        }
    };
    
    private static readonly JsonSerializerSettings AlbumInfluenceJsonOptions = new()
    {
        Converters =
        {
            new AlbumInfluenceNodeConverter(),
            new JsonToOutModelConverter<OutGenre>("genre"),
            new JsonToOutModelConverter<OutAlbumInfluence>("album_influence")
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
                ReleaseMapperJsonOptions)
            : null,

        PrimaryGenres = album.TryGetValue("primary_genres", out var primaryGenreValue)
                        && primaryGenreValue is not null
            ? JsonConvert.DeserializeObject<AlbumGenreNode[]>(primaryGenreValue as string ?? string.Empty,
                AlbumGenreJsonOptions)
            : null,

        InfluenceGenres = album.TryGetValue("influence_genres", out var influenceGenreValue)
                          && influenceGenreValue is not null
            ? JsonConvert.DeserializeObject<AlbumInfluenceNode[]>(influenceGenreValue as string ?? string.Empty,
                AlbumInfluenceJsonOptions)
            : null,
        
        Stats = album.SafeGet<long?>(AlbumRatingStatColumns.AlbumId) != null
            ? new OutAlbumRatingStat
            {
                Average = album.SafeGet<decimal>(AlbumRatingStatColumns.Average),
                Count = album.SafeGet<int>(AlbumRatingStatColumns.Count),
                Sum = album.SafeGet<long>(AlbumRatingStatColumns.Sum),
            }
            : null,

        ArtistAlias = album.SafeGet<string>(AlbumColumns.ArtistAlias),
        ArtworkUrl = album.SafeGet<string>(AlbumColumns.ArtworkUrl),
        BeginRecordDate = album.SafeGet<DateTime?>(AlbumColumns.BeginRecordDate),
        DiscTotal = album.SafeGet<int>(AlbumColumns.DiscTotal),
        EndRecordDate = album.SafeGet<DateTime?>(AlbumColumns.EndRecordDate),
        EnglishName = album.SafeGet<string>(AlbumColumns.EnglishName),
        IsExplicitContent = album.SafeGet<bool>(AlbumColumns.IsExplicitContent),
        IsFarRight = album.SafeGet<bool>(AlbumColumns.IsFarRight),
        IsGraphicContent = album.SafeGet<bool>(AlbumColumns.IsGraphicContent),
        IsVisible = album.SafeGet<bool>(AlbumColumns.IsVisible),
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
        
        ArtistAlias = album.ArtistAlias,
        ArtworkUrl = album.ArtworkUrl,
        BeginRecordDate = album.BeginRecordDate,
        DiscTotal = album.DiscTotal,
        EndRecordDate = album.EndRecordDate,
        EnglishName = album.EnglishName,
        IsExplicitContent = album.IsExplicitContent,
        IsFarRight = album.IsFarRight,
        IsGraphicContent = album.IsGraphicContent,
        IsVisible = album.IsVisible,
        Language = album.Language,
        Name = album.Name,
        OriginalReleaseDate = album.OriginalReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };
}