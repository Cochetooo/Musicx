using log4net;
using Musicx.Core.Interfaces;
using Musicx.Core.Logging;
using Musicx.Core.Models;
using Musicx.Data.Entities;
using Musicx.Infrastructure.Loaders;
using Musicx.Infrastructure.Managers;

namespace Musicx.Infrastructure.Mappers;

public interface IAlbumMapper : IMapper<Album, AlbumEntity>;

public class AlbumMapper(ILoggerFactory loggerFactory) : IAlbumMapper
{
    private readonly ILogger<AlbumMapper> Logger = loggerFactory.CreateLogger<AlbumMapper>();
    
    // Mappage de l'entité vers le DTO
    public Album ToDto(AlbumEntity albumEntity)
    {
        // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
        var genreIds = albumEntity.Genres?.Select(g => g.GenreId).ToList() ?? [];
        var influenceGenreIds = albumEntity.InfluenceGenres?.Select(g => g.GenreId).ToList() ?? [];

        var albumDto = new Album
        {
            Id = albumEntity.Id,
            
            GenreIds = genreIds,
            InfluenceGenreIds = influenceGenreIds,
            
            ArtistId = albumEntity.ArtistId,
            ArtworkUrl = albumEntity.ArtworkUrl,
            CatalogNumber = albumEntity.CatalogNumber,
            DiscTotal = albumEntity.DiscTotal,
            LabelId = albumEntity.LabelId,
            Name = albumEntity.Name,
            ReleaseDate = albumEntity.ReleaseDate,
            ReleaseType = albumEntity.ReleaseType,
            TrackTotal = albumEntity.TrackTotal,
        };

        return albumDto;
    }

    // Mappage du DTO vers l'entité
    public AlbumEntity ToEntity(Album albumDto)
    {
        var entity = new AlbumEntity();
        
        // On crée les entités de relation pour chaque liste d'ID
        var genres = albumDto.GenreIds.Select(genreId => new AlbumGenreEntity { GenreId = genreId }).ToList();
        var influenceGenres = albumDto.InfluenceGenreIds.Select(genreId => new AlbumInfluenceGenreEntity { GenreId = genreId }).ToList();
        
        entity.Id = albumDto.Id;
        
        entity.Genres = genres;
        entity.InfluenceGenres = influenceGenres;
        
        entity.ArtistId = albumDto.ArtistId;
        entity.ArtworkUrl = albumDto.ArtworkUrl;
        entity.CatalogNumber = albumDto.CatalogNumber;
        entity.DiscTotal = albumDto.DiscTotal;
        entity.LabelId = albumDto.LabelId;
        entity.Name = albumDto.Name;
        entity.ReleaseDate = albumDto.ReleaseDate;
        entity.ReleaseType = albumDto.ReleaseType;
        entity.TrackTotal = albumDto.TrackTotal;

        return entity;
    }

}