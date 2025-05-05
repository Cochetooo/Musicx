using Musicx.Contracts.Dto.Responses;
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
}