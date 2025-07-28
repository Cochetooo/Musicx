using System.Collections.Immutable;
using System.Dynamic;
using System.Text.Json;
using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Contracts.Helpers;
using Musicx.Domain.Enums;


namespace Musicx.Contracts.Mappers;

public static class AlbumMapper
{
    public static OutAlbum FromDicoToAlbum(this IDictionary<string, object?> album) => new() 
    {
        Id = album.SafeGet<long>("album_id"),
        
        CreatedAt = album.SafeGet<DateTime>("album_created_at"),
        UpdatedAt = album.SafeGet<DateTime>("album_updated_at"),
        
        Artist = album.SafeGet<long?>("album_artist_id") != null
            ? new OutArtist
            {
                Id = album.SafeGet<long>("artist_id"),
                Name = album.SafeGet<string>("artist_name") ?? "",
            }
            : null,
        
        Releases = album.TryGetValue("releases", out var releaseValue)
            ? JsonSerializer.Deserialize<OutRelease[]>(releaseValue as string ?? string.Empty)
            : [],
        
        PrimaryGenres = album.TryGetValue("primary_genres", out var primaryGenreValue)
            ? JsonSerializer.Deserialize<OutGenre[]>(primaryGenreValue as string ?? string.Empty)
            : null,
        
        InfluenceGenres = album.TryGetValue("influence_genres", out var influenceGenreValue)
            ? JsonSerializer.Deserialize<OutGenre[]>(influenceGenreValue as string ?? string.Empty)
            : null,
        
        ArtworkUrl = album.SafeGet<string>("album_artwork_url"),
        DiscTotal = album.SafeGet<int>("album_disc_total"),
        IsFarRight = album.SafeGet<bool>("album_is_far_right"),
        Name = album.SafeGet<string>("album_name") ?? "",
        ReleaseDate = album.SafeGet<DateTime>("album_original_release_date"),
        ReleaseType = album.SafeGet<ReleaseType>("album_original_release_type"),
        TrackTotal = album.SafeGet<int>("album_track_total"),
    };

    public static InAlbum ToRaw(this OutAlbum album) => new()
    {
        Id = album.Id,
        ArtistId = album.Artist?.Id,
        ReleaseIds = album.Releases?.Select(r => r.Id).ToList(),
        PrimaryGenreIds = album.PrimaryGenres?.Select(g => g.Id).ToList(),
        InfluenceGenreIds = album.InfluenceGenres?.Select(g => g.Id).ToList(),
        ArtworkUrl = album.ArtworkUrl,
        DiscTotal = album.DiscTotal,
        IsFarRight = album.IsFarRight,
        Name = album.Name,
        ReleaseDate = album.ReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };
}