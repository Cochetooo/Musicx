using log4net;
using MusicxApi.Models;

namespace Musicx.Entities.Mappers;

public static class GenreMapper
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(GenreMapper));
    
    // Mappage de l'entité vers le DTO
    public static Genre? ToDto(this GenreEntity? genreEntity)
    {
        if (null == genreEntity)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        var genreDto = new Genre
        {
            Id = genreEntity.Id,
            Name = genreEntity.Name
        };

        // Mappage des Parents et des Children en termes d'IDs
        genreDto.ParentIds = genreEntity.Parents.Select(p => p.ParentId).ToList();
        genreDto.ChildIds = genreEntity.Children.Select(c => c.ChildId).ToList();

        return genreDto;
    }

    // Mappage du DTO vers l'entité
    public static GenreEntity? ToEntity(this Genre? genreDto)
    {
        if (null == genreDto)
        {
            Logger.Warn("⚠️ Entity is null.");
            return null;
        }
        
        var genreEntity = new GenreEntity
        {
            Id = genreDto.Id,
            Name = genreDto.Name
        };

        // Mappage des Parents et des Children (en créant les relations Many-to-Many)
        genreEntity.Parents = genreDto.ParentIds.Select(parentId => new GenreParentEntity
        {
            ParentId = parentId,
            ChildId = genreEntity.Id  // Relier cet enfant à ce parent
        }).ToList();

        genreEntity.Children = genreDto.ChildIds.Select(childId => new GenreParentEntity
        {
            ParentId = genreEntity.Id,  // Relier ce parent à cet enfant
            ChildId = childId
        }).ToList();

        return genreEntity;
    }
}
