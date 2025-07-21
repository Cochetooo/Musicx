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
        
        Releases = album.Releases.Select(r => new OutRelease { Id = r.Id, CatalogNumber = r.CatalogNumber }).ToList(),
        PrimaryGenres = album.PrimaryGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),
        InfluenceGenres = album.InfluenceGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),
        
        ArtworkUrl = album.ArtworkUrl,
        DiscTotal = album.DiscTotal,
        IsFarRight = album.IsFarRight,
        Name = album.Name,
        ReleaseDate = album.ReleaseDate,
        ReleaseType = album.ReleaseType,
        TrackTotal = album.TrackTotal
    };

    public static Album ToEntity(this InAlbum albumDto)
    {
        return new Album
        {
            Id = albumDto.Id,

            ArtistId = albumDto.ArtistId,

            Releases = albumDto.ReleaseIds.Select(id => new Release { Id = id }).ToList(),
            PrimaryGenres = albumDto.PrimaryGenreIds.Select(id => new Genre { Id = id }).ToList(),
            InfluenceGenres = albumDto.InfluenceGenreIds.Select(id => new Genre { Id = id }).ToList(),

            ArtworkUrl = albumDto.ArtworkUrl,
            DiscTotal = albumDto.DiscTotal,
            IsFarRight = albumDto.IsFarRight,
            Name = albumDto.Name,
            ReleaseDate = albumDto.ReleaseDate,
            ReleaseType = albumDto.ReleaseType,
            TrackTotal = albumDto.TrackTotal
        };
    }

    public static InAlbum ToRaw(this OutAlbum album) => new()
    {
        Id = album.Id,
        ArtistId = album.Artist?.Id ?? null,
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
}