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
    public static OutAlbum ToDto(this IDictionary<string, object?> album) => new()
    {
        Id = (long) album["album_id"]!,
        
        CreatedAt = (DateTime) album["created_at"]!,
        UpdatedAt = (DateTime) album["updated_at"]!,
        
        Artist = album["artist_id"] != null
            ? new OutArtist
            {
                Id = (long) album["artist_id"]!,
                Name = album["artist_name"]?.ToString() ?? "",
            }
            : null,
        
        Releases = album.ContainsKey("releases")
            ? JsonSerializer.Deserialize<OutRelease[]>(album["releases"] as string ?? string.Empty)
            : [],
        
        PrimaryGenres = album.ContainsKey("primary_genres")
            ? JsonSerializer.Deserialize<OutGenre[]>(album["primary_genres"] as string ?? string.Empty)
            : null,
        
        InfluenceGenres = album.ContainsKey("influence_genres")
            ? JsonSerializer.Deserialize<OutGenre[]>(album["influence_genres"] as string ?? string.Empty)
            : null,
        
        ArtworkUrl = album["album_artwork_url"]?.ToString(),
        DiscTotal = (int?)album["album_disc_total"],
        IsFarRight = (bool) album["album_is_far_right"]!,
        Name = album["album_name"]?.ToString() ?? "",
        ReleaseDate = (DateTime?)album["album_original_release_date"],
        ReleaseType = (ReleaseType?)album["album_release_type"],
        TrackTotal = (int?)album["album_track_total"],
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