using System.Collections.Immutable;
using System.Dynamic;
using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Domain.Enums;
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
            : new OutArtist
            {
                Id = album.SafeGet<long>(AlbumColumns.ArtistId),
                Name = "",
            },

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
        Language = album.SafeGet<string>(AlbumColumns.Language),
        Name = album.SafeGet<string>(AlbumColumns.Name) ?? "",
        OriginalReleaseDate = album.SafeGet<DateTime?>(AlbumColumns.OriginalReleaseDate),
        ReleaseType = album.SafeGet<ReleaseType?>(AlbumColumns.ReleaseType),
        TrackTotal = album.SafeGet<int>(AlbumColumns.TrackTotal),
    };
    
    public static T? DebugDeserialization<T>(string json, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.Parse(json);

        Console.WriteLine("---- START DEBUG ----");
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            foreach (var prop in element.EnumerateObject())
            {
                var converted = options.PropertyNamingPolicy?.ConvertName(prop.Name);
                Console.WriteLine($"JSON: {prop.Name} -> Policy: {converted}");
            }
        }
        Console.WriteLine("---- END DEBUG ----");

        try
        {
            Console.WriteLine(json);
            var result = JsonSerializer.Deserialize<T>(json, options);
            Console.WriteLine("Désérialisation OK.");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine("EXCEPTION !");
            Console.WriteLine(ex);
            throw;
        }
    }

    public static InAlbum ToRaw(this OutAlbum album) => new()
    {
        Id = album.Id,
        
        ArtistId = album.Artist?.Id,
        ReleaseIds = album.Releases?.Select(r => r.Id).ToList(),
        PrimaryGenreIds = album.PrimaryGenres?.Select(g => g.Id).ToList(),
        InfluenceGenreIds = album.InfluenceGenres?.Select(g => g.Id).ToList(),
        
        ArtworkUrl = album.ArtworkUrl,
        BeginRecordDate = album.BeginRecordDate,
        DiscTotal = album.DiscTotal,
        EndRecordDate = album.EndRecordDate,
        IsFarRight = album.IsFarRight,
        Language = album.Language,
        Name = album.Name,
        OriginalReleaseDate = album.OriginalReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };
}