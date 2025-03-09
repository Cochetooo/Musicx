using log4net;
using Musicx.Core.Models;
using Musicx.Data.Entities;

namespace Musicx.Infrastructure.Mappers;

public static class AlbumMapper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(AlbumMapper));
    
    // Mappage de l'entité vers le DTO
    public static Album? ToDto(this AlbumEntity? albumEntity)
    {
        if (null == albumEntity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }

        // On récupère les identifiants des relations (Lazy Loading pour les relations Many-to-Many)
        var genreIds = albumEntity.Genres?.Select(g => g.GenreId).ToList() ?? new List<ulong>();
        var influenceGenreIds = albumEntity.InfluenceGenres?.Select(g => g.GenreId).ToList() ?? new List<ulong>();

        return new Album
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
    }

    // Mappage du DTO vers l'entité
    public static void FromDto(this AlbumEntity entity, Album albumDto)
    {
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
    }
}