using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Enums;
using Musicx.Contracts.Helpers;
using Musicx.Infrastructure.API.Persistence.Columns;
using Newtonsoft.Json;


namespace Musicx.Infrastructure.API.Persistence.Mappers;

public static class SongMapper
{
    private static readonly JsonSerializerSettings GenreMapperJsonOptions = new()
    {
        Converters =
        {
            new JsonToOutModelConverter<OutGenre>("genre")
        }
    };
    
    public static OutSong FromDicoToSong(this IDictionary<string, object?> song) => new()
    {
        Id = song.SafeGet<long>(SongColumns.Id),

        CreatedAt = song.SafeGet<DateTime>(AlbumColumns.CreatedAt),
        UpdatedAt = song.SafeGet<DateTime>(AlbumColumns.UpdatedAt),

        Album = song.SafeGet<long?>(AlbumColumns.Id) != null
            ? song.FromDicoToAlbum()
            : new OutAlbum
            {
                Id = song.SafeGet<long>(AlbumColumns.Id),
                Name = "",
            },
        
        Artist = song.SafeGet<long?>(ArtistColumns.Id) != null
            ? song.FromDicoToArtist()
            : new OutArtist
            {
                Id = song.SafeGet<long>(ArtistColumns.Id),
                Name = "",
            },
        
        PrimaryGenres = song.TryGetValue("primary_genres", out var primaryGenreValue)
                        && primaryGenreValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(primaryGenreValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,

        InfluenceGenres = song.TryGetValue("influence_genres", out var influenceGenreValue)
                          && influenceGenreValue is not null
            ? JsonConvert.DeserializeObject<OutGenre[]>(influenceGenreValue as string ?? string.Empty,
                GenreMapperJsonOptions)
            : null,

        DiscNumber = song.SafeGet<int>(SongColumns.DiscNumber),
        Duration = song.SafeGet<long>(SongColumns.Duration),
        Lyrics = song.SafeGet<string>(SongColumns.Lyrics),
        Title = song.SafeGet<string>(SongColumns.Title) ?? "",
        TrackNumber = song.SafeGet<int>(SongColumns.TrackNumber),
        Type = song.SafeGet<SongType>(SongColumns.Type)
    };

    public static InSong ToRaw(this OutSong song) => new()
    {
        Id = song.Id,
        
        AlbumId = song.Album?.Id,
        ArtistId = song.Artist?.Id,
        PrimaryGenreIds = song.PrimaryGenres?.Select(g => g.Id).ToList(),
        InfluenceGenreIds = song.InfluenceGenres?.Select(g => g.Id).ToList(),

        DiscNumber = song.DiscNumber,
        Duration = song.Duration,
        Lyrics = song.Lyrics,
        Title = song.Title,
        TrackNumber = song.TrackNumber,
        Type = song.Type,
    };
}