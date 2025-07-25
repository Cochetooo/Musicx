using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Domain.Enums;
using Musicx.Domain.Models;

namespace Musicx.Contracts.Mappers;

public static class AlbumMapper
{
    public static OutAlbum ToDto(this Album album) => new()
    {
        Id = album.Id,
        
        CreatedAt = album.CreatedAt,
        UpdatedAt = album.UpdatedAt,
        
        Artist = album.Artist?.ToDto(),
        
        Releases = album.Releases.Select(r => new OutRelease { Id = r.Id }).ToList(),
        PrimaryGenres = album.PrimaryGenres.Select(g => new OutGenre { Id = g.Id }).ToList(),
        InfluenceGenres = album.InfluenceGenres.Select(g => new OutGenre { Id = g.Id }).ToList(),
        
        ArtworkUrl = album.ArtworkUrl,
        DiscTotal = album.DiscTotal,
        IsFarRight = album.IsFarRight,
        Name = album.Name,
        ReleaseDate = album.ReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };

    public static Album ToEntity(this InAlbum albumDto) => new()
    {
        Id = albumDto.Id,

        Artist = albumDto.ArtistId is null ? null : ProxyArtist(albumDto.ArtistId.Value),

        Releases = albumDto.ReleaseIds.Select(ProxyRelease).ToList(),
        PrimaryGenres = albumDto.PrimaryGenreIds.Select(ProxyGenre).ToList(),
        InfluenceGenres = albumDto.InfluenceGenreIds.Select(ProxyGenre).ToList(),

        ArtworkUrl = albumDto.ArtworkUrl,
        DiscTotal = albumDto.DiscTotal,
        IsFarRight = albumDto.IsFarRight,
        Name = albumDto.Name,
        ReleaseDate = albumDto.ReleaseDate,
        ReleaseType = albumDto.ReleaseType,
        TrackTotal = albumDto.TrackTotal
    };

    public static InAlbum ToRaw(this OutAlbum album) => new()
    {
        Id = album.Id,
        ArtistId = album.Artist?.Id,
        ReleaseIds = album.Releases.Select(r => r.Id).ToList(),
        PrimaryGenreIds = album.PrimaryGenres.Select(g => g.Id).ToList(),
        InfluenceGenreIds = album.InfluenceGenres.Select(g => g.Id).ToList(),
        ArtworkUrl = album.ArtworkUrl,
        DiscTotal = album.DiscTotal,
        IsFarRight = album.IsFarRight,
        Name = album.Name,
        ReleaseDate = album.ReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };
    
    private static Artist ProxyArtist(long id) => new()
    {
        Id = id,
        Name = string.Empty
    };
    
    private static Release ProxyRelease(long id) => new()
    {
        Id = id,
        CatalogNumber = string.Empty
    };
    
    private static Genre ProxyGenre(long id) => new()
    {
        Id = id,
        Name = string.Empty
    };
}