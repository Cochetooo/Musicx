using Musicx.Contracts.Dto.Requests;
using Musicx.Contracts.Dto.Responses;
using Musicx.Domain.Enums;
using Musicx.Domain.Models;

namespace Musicx.Contracts.Mappers;

public static class AlbumMapper
{
    public static OutAlbum ToDto(this Album album) => new OutAlbum
    {
        Id = album.Id,
        
        Artist = album.Artist?.ToDto(),
        
        Releases = album.Releases.Select(r => new OutRelease { Id = r.Id, CatalogNumber = r.CatalogNumber }).ToList(),
        PrimaryGenres = album.PrimaryGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),
        InfluenceGenres = album.InfluenceGenres.Select(g => new OutGenre { Id = g.Id, Name = g.Name }).ToList(),
        
        ArtworkUrl = album.ArtworkUrl,
        DiscTotal = album.DiscTotal,
        Name = album.Name,
        ReleaseDate = album.ReleaseDate,
        ReleaseType = album.ReleaseType.ToString(),
        TrackTotal = album.TrackTotal
    };

    public static Album ToEntity(this InAlbum albumDto)
    {
        var releaseType = Enum.TryParse<ReleaseType>(albumDto.ReleaseType, out var cReleaseType)
            ? cReleaseType
            : ReleaseType.Unknown;

        return new Album
        {
            Id = albumDto.Id,

            Artist = albumDto.ArtistId is null ? null : new Artist { Id = albumDto.ArtistId.Value },

            Releases = albumDto.ReleaseIds.Select(id => new Release { Id = id }).ToList(),
            PrimaryGenres = albumDto.PrimaryGenreIds.Select(id => new Genre { Id = id }).ToList(),
            InfluenceGenres = albumDto.InfluenceGenreIds.Select(id => new Genre { Id = id }).ToList(),

            ArtworkUrl = albumDto.ArtworkUrl,
            DiscTotal = albumDto.DiscTotal,
            Name = albumDto.Name,
            ReleaseDate = albumDto.ReleaseDate,
            ReleaseType = releaseType,
            TrackTotal = albumDto.TrackTotal
        };
    }
}